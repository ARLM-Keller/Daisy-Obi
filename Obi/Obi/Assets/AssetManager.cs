using System;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace Obi.Assets
{
	public class AssetManager
	{
		// member variables
		// hold path of project directory
		
		private string m_sDirPath;

		internal string DirPath
		{
			get
			{
				return m_sDirPath ;
			}
		}

        private Uri mBaseURI;

        /// <summary>
        /// Base URI of the asset manager directory.
        /// </summary>
        public Uri BaseURI
        {
            get
            {
                return mBaseURI;
            }
        }
		
		//hash table to hold paths of assets being managed
		private Hashtable m_htAssetList = new Hashtable();


		// hash table to contain list of all existing assets
		public Hashtable m_htExists  = new Hashtable ();

		// object for catch class
		//CatchEvents ob_Catch = new CatchEvents();

		/// <summary>
		/// Create the asset manager taking as argument the project directory where the data should live.
		/// The directory is created if it didn't exist; an exception is raised if a problem occurs.
		/// </summary>
		public AssetManager(string projectDirectory)
		{
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "file";
            builder.Path = projectDirectory + @"\";
            mBaseURI = builder.Uri;
            m_sDirPath = System.Text.RegularExpressions.Regex.Replace(mBaseURI.LocalPath, @"^\\\\localhost\\", "");
			if (!Directory.Exists(m_sDirPath))
			{
				try
				{
					Directory.CreateDirectory(m_sDirPath);
				}
				catch (Exception e)
				{
					throw new Exception(String.Format("Could not create project directory {0}", m_sDirPath), e);
				}
			}
		}

		/// <summary>
		/// Create a new empty AudioMediaAsset object with the given parameters and add it to the list of managed assets.
		/// </summary>
		/// <param name="channels">Number of channels</param>
		/// <param name="bitDepth">Bit depth</param>
		/// <param name="sampleRate">Sample rate</param>
		/// <returns>The newly created asset.</returns>
		public AudioMediaAsset NewAudioMediaAsset(int channels, int bitDepth, int sampleRate)
		{
			AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset (channels , bitDepth , sampleRate) ;
			ob_AudioMediaAsset.Name = NewMediaAssetName () ;
			ob_AudioMediaAsset.m_AssetManager = this ;
			m_htAssetList.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;
			m_htExists.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;   
			return ob_AudioMediaAsset ;
		}

		/// <summary>
		/// Create a new AudioMediaAsset object from a list of clips and add it to the list of managed assets.
		/// </summary>
		/// <param name="clips">The array of <see cref="AudioClip"/>s.</param>
		/// <returns>The newly created asset.</returns>
		public AudioMediaAsset NewAudioMediaAsset(ArrayList clips)
		{
			
			AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset (clips) ;

			ob_AudioMediaAsset.Name = NewMediaAssetName () ;
			ob_AudioMediaAsset.m_AssetManager = this ;
			m_htAssetList.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;
			m_htExists.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;   
			return ob_AudioMediaAsset ;
			

		}

		/// <summary>
		/// Produce a unique name for IMediaAsset
		/// </summary>
		public string NewMediaAssetName()
		{

			long i = 0 ;

			string sTemp ;

			sTemp = "amMediaAsset" ;
			string sTempName ;
			sTempName = sTemp + i.ToString () ;

			while ( m_htExists.ContainsKey (sTempName)  && i<9000000)
			{

				i++;
				sTempName = sTemp + i.ToString () ;
				

			}


			if (i<9000000)

			{
			
				return sTempName ;

			}
			else
			{
				return null ;
			}
		}
		
			
		

		#region IAssetManager Members

		public Hashtable Assets
		{
			get
			{
				return m_htAssetList ;
			}
		}

		public void AddAsset(Assets.MediaAsset asset)
		{
			if (asset.Type== MediaType.Audio)
			{
				AudioMediaAsset Asset  = asset as AudioMediaAsset;
				if (Asset.Name == null )
				{
					Asset.Name = NewMediaAssetName () ;
				}
				Asset.m_AssetManager = this ;

				m_htAssetList.Add (Asset.Name, asset) ;
				m_htExists.Add (Asset.Name, asset) ;
			}
		}

		public Hashtable GetAssets(Assets.MediaType assetType)
		{

			IDictionaryEnumerator en = m_htAssetList.GetEnumerator();
			
			Hashtable htTemp = new Hashtable () ;

			MediaAsset m ;
			//find the asset from hash table using key 
			while (en.MoveNext() )
			{
				m =en.Value as MediaAsset ;

				if (m.Type.Equals (assetType)) 
				{
					htTemp.Add ( en.Key , en.Value) ; 
				}
			}		

			return htTemp ;
		}
	

		public Assets.MediaAsset GetAsset(string assetName)
		{
			if (m_htAssetList.ContainsKey (assetName))
			{
				return m_htAssetList [assetName] as MediaAsset;
			}
			else
				throw new Exception ("Asset not found in Hashtable") ;
						
						
		}

		public Assets.MediaAsset NewAsset(Assets.MediaType assetType)
		{
			return null;
		}

		public void DeleteAsset(Assets.MediaAsset assetToDelete)
		{
		
			if (  m_htAssetList.ContainsKey(assetToDelete.Name) )
			{
				m_htAssetList.Remove(assetToDelete.Name) ;
				m_htExists.Remove(assetToDelete.Name) ;
				assetToDelete = null ;
			}
			else
			{

				throw new Exception ("Asset could not be deleted : not in hashtable") ;
			}

		}

		public void RemoveAsset(Assets.MediaAsset assetToRemove)
		{
			MediaAsset MediaAssetToRemove = assetToRemove as MediaAsset ;
			if (  m_htAssetList.ContainsKey(MediaAssetToRemove.Name) )
			{
				m_htAssetList.Remove(MediaAssetToRemove.Name) ;
				MediaAssetToRemove.m_AssetManager = null ;

			}
			else
				throw new Exception ("Asset could not be removed : not in hashtable") ;
			
		}

		public Assets.MediaAsset CopyAsset(Assets.MediaAsset asset)
		{
			if (m_htAssetList.ContainsKey (asset.Name)  )
			{
				MediaAsset ob_MediaAsset = asset.Copy()  as MediaAsset;
				ob_MediaAsset.Name = NewMediaAssetName () ;
				m_htAssetList.Add (ob_MediaAsset.Name, ob_MediaAsset) ;
				m_htExists.Add (ob_MediaAsset.Name, ob_MediaAsset) ;
				return ob_MediaAsset ;
			}
			else
			{
				throw new Exception ("Asset not found in Hashtable") ;

			}

		}


		public string RenameAsset(Assets.MediaAsset asset, String newName)
		{
			string OldName = asset.Name;
			bool boolRenamed = false ;
			IDictionaryEnumerator enRemove = m_htAssetList.GetEnumerator();
			while(enRemove.MoveNext())
			{

				if(enRemove.Key.ToString() == asset.Name)
				{
					m_htAssetList.Remove(OldName );
					m_htExists.Remove(OldName );
					asset.Name = newName;
					m_htAssetList.Add(asset.Name, asset);
					m_htExists.Add(asset.Name, asset);
					boolRenamed = true ;
					break ;
				}
			}
			if (boolRenamed == false)
				throw new Exception ("Asset cannot be renamed : not in hashtable") ;
			return OldName;
		}

        /// <summary>
        /// Create an asset directly from a file and add it into the manager.
        /// Its file is copied to the asset manager directory.
        /// </summary>
        /// <param name="path">The path of the file to import.</param>
        /// <returns>The asset created.</returns>
        public AudioMediaAsset ImportAudioMediaAsset(string path)
        {
            ArrayList clips = new ArrayList(1);
            clips.Add(AudioClip.ImportClip(path, this));
            return NewAudioMediaAsset(clips);
        }
	}

	#endregion


}

