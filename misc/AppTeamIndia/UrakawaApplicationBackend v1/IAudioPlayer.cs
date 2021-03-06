using System;
using System.Collections;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.AudioVideoPlayback;

namespace urakawaApplication
{
    public interface IAudioPlayer
    {
        // throws AudioPlayerException
        //parameter is "OutputDevice" not Object
        //void setOutputDevice(Object device);

        //return value should be OutputDevice not Object
        //Object getCurrentOutputDevice();

        //throws AudioPlayerException
        void play(IAudioMediaAsset wave);
        //throws AudioPlayerException
        //void play(IAudioMediaAsset wave, double bytePositionToStartFrom);
        // throws AudioPlayerException
        void play(IAudioMediaAsset wave, double timePositionToStartFrom);
        // throws AudioPlayerException 
        //parameter should be ByteBuffer not Object
        //void play(Object buf);
        void stopPlaying();

        //returns current position in bytes of current AudioMediaAsset
        //double getCurrentBytePosition(); 
        //returns current position in time (milliseconds) of current AudioMediaAsset
      double getCurrentTimePosition(); 

        //the return type will be something like VuMeterStateType instead of Object
        //Object getState();

    }
	class AudioPlayer : IAudioPlayer
	{
		

		public Audio ob_Audio ;
		
		public void play(IAudioMediaAsset wave)
		{
			string m_sPath = null;
			ob_Audio = new Audio(m_sPath);
			ob_Audio.Play ();
		}
		
		public void play(IAudioMediaAsset wave, double timePositionToStartFrom)
		{
			string m_sPath = null;
			ob_Audio = new Audio(m_sPath);			
			ob_Audio.CurrentPosition = timePositionToStartFrom ;
			ob_Audio.Play ();
		}
		public         void stopPlaying()
		{
			ob_Audio.Stop();
		}
		public double getCurrentTimePosition()
	{
return ob_Audio.CurrentPosition ;
	}
	}
}
