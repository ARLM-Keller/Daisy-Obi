using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using UrakawaApplicationBackend.events.vuMeterEvents ;
using Microsoft.DirectX ;
using Microsoft.DirectX.DirectSound ;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// Summary description for VuMeterForm.
	/// </summary>
	public class VuMeterForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		public VuMeterForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tmRefresh = new System.Windows.Forms.Timer(this.components);
			this.txtOverloadLeft = new System.Windows.Forms.TextBox();
			this.txtOverloadRight = new System.Windows.Forms.TextBox();
			this.txtAmplitudeLeft = new System.Windows.Forms.TextBox();
			this.txtAmplitudeRight = new System.Windows.Forms.TextBox();
			this.tmBeep = new System.Windows.Forms.Timer(this.components);
			this.tmRefreshText = new System.Windows.Forms.Timer(this.components);
			this.btnClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tmRefresh
			// 
			this.tmRefresh.Tick += new System.EventHandler(this.tmRefresh_Tick);
			// 
			// txtOverloadLeft
			// 
			this.txtOverloadLeft.AccessibleName = "OverloadLeft";
			this.txtOverloadLeft.Location = new System.Drawing.Point(424, 240);
			this.txtOverloadLeft.Name = "txtOverloadLeft";
			this.txtOverloadLeft.TabIndex = 0;
			this.txtOverloadLeft.Text = "";
			// 
			// txtOverloadRight
			// 
			this.txtOverloadRight.AccessibleName = "Overload Right";
			this.txtOverloadRight.Enabled = false;
			this.txtOverloadRight.Location = new System.Drawing.Point(424, 296);
			this.txtOverloadRight.Name = "txtOverloadRight";
			this.txtOverloadRight.ReadOnly = true;
			this.txtOverloadRight.TabIndex = 1;
			this.txtOverloadRight.Text = "";
			// 
			// txtAmplitudeLeft
			// 
			this.txtAmplitudeLeft.AccessibleName = "Left Channel Amplitude";
			this.txtAmplitudeLeft.Location = new System.Drawing.Point(424, 32);
			this.txtAmplitudeLeft.Name = "txtAmplitudeLeft";
			this.txtAmplitudeLeft.ReadOnly = true;
			this.txtAmplitudeLeft.TabIndex = 2;
			this.txtAmplitudeLeft.Text = "";
			// 
			// txtAmplitudeRight
			// 
			this.txtAmplitudeRight.AccessibleName = "Right Channel Amplitude";
			this.txtAmplitudeRight.Location = new System.Drawing.Point(424, 64);
			this.txtAmplitudeRight.Name = "txtAmplitudeRight";
			this.txtAmplitudeRight.ReadOnly = true;
			this.txtAmplitudeRight.TabIndex = 3;
			this.txtAmplitudeRight.Text = "";
			// 
			// tmBeep
			// 
			this.tmBeep.Enabled = true;
			this.tmBeep.Interval = 1000;
			this.tmBeep.Tick += new System.EventHandler(this.tmBeep_Tick);
			// 
			// tmRefreshText
			// 
			this.tmRefreshText.Enabled = true;
			this.tmRefreshText.Interval = 1000;
			this.tmRefreshText.Tick += new System.EventHandler(this.tmRefreshText_Tick);
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(496, 496);
			this.btnClose.Name = "btnClose";
			this.btnClose.TabIndex = 4;
			this.btnClose.Text = "&Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// VuMeterForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 566);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.txtAmplitudeRight);
			this.Controls.Add(this.txtAmplitudeLeft);
			this.Controls.Add(this.txtOverloadRight);
			this.Controls.Add(this.txtOverloadLeft);
			this.Name = "VuMeterForm";
			this.Text = "VuMeterForm";
			this.Load += new System.EventHandler(this.VuMeterForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

internal int HighTop =0 ;
		internal int HighBottom = 0 ;

		internal int NormalTop = 0 ;
		internal int NormalBottom = 0 ;

		internal int LowTop = 0 ;
		internal int LowBottom = 0 ;

internal int LineWidth = 0 ;

internal int ScaleFactor = 1 ;
		internal int LeftGraphX = 0 ;
internal int RightGraphX = 0 ;

internal int BackGroundWidth = 0 ;
		internal int BackGroundTop = 0 ;
internal int BackGroundBottom = 0 ;
internal int BackGroundX ;

internal int EraserLeft = 0 ;
internal int EraserRight = 0 ;

internal int PeakOverloadLightX  =0 ;
		internal int PeakOverloadLightY  =0 ;
		

private int AmplitudeLeft = 0 ;
		private int AmplitudeRight = 0 ;

VuMeter ob_VuMeter ;



		private System.Windows.Forms.Timer tmRefresh;
		private System.Windows.Forms.TextBox txtOverloadLeft;
		private System.Windows.Forms.TextBox txtOverloadRight;
		internal System.Windows.Forms.TextBox txtAmplitudeLeft;
		internal System.Windows.Forms.TextBox txtAmplitudeRight;
		private System.Windows.Forms.Timer tmRefreshText;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Timer tmBeep;
		

// initialise the form and frame for graph display
		private void VuMeterForm_Load(object sender, System.EventArgs e)
		{
			System.Drawing.Graphics objGraphics;
			objGraphics = this.CreateGraphics();		

			Pen PenWhite = new Pen(Color.White );
			PenWhite.Width = BackGroundWidth;
			objGraphics.DrawLine(PenWhite , BackGroundX , BackGroundTop , BackGroundX, BackGroundBottom ) ;		

// enables the refresh timer for repainting graph at regular interval
tmRefresh.Enabled = true ;
			
		}

// load the beep file and plays it once
		void LoadBeep ()
		{
Device BeepDevice  = new Device() ;
			BeepDevice.SetCooperativeLevel(this, CooperativeLevel.Normal );
	
									SecondaryBuffer BeepBuffer  =  new SecondaryBuffer("c:\\beep.wav", BeepDevice );
			

BeepBuffer.Play(0, BufferPlayFlags.Default );		
			
		}

// function to catch the update event from VuMeter class to update graph cordinates
		public void CatchUpdateForms (VuMeter ob_VuMeterArg , UpdateForms Update )
		{
ob_VuMeter = ob_VuMeterArg ;

			// Update cordinates
			HighTop =ob_VuMeter.Graph.HighTop ;
			HighBottom = ob_VuMeter.Graph.HighBottom ;

			NormalTop = ob_VuMeter.Graph.NormalTop ;
			NormalBottom = ob_VuMeter.Graph.NormalBottom ;

			LowTop = ob_VuMeter.Graph.LowTop ;
			LowBottom = ob_VuMeter.Graph.LowBottom ;

			LineWidth = ob_VuMeter.Graph.LineWidth ;

			LeftGraphX = ob_VuMeter.Graph.LeftGraphX ;
			RightGraphX = ob_VuMeter.Graph.RightGraphX ;

			BackGroundWidth = ob_VuMeter.Graph.BackGroundWidth ;
			BackGroundTop = ob_VuMeter.Graph.BackGroundTop ;
			BackGroundBottom = ob_VuMeter.Graph.BackGroundBottom ;

			EraserLeft = ob_VuMeter.Graph.EraserLeft ;
			EraserRight = ob_VuMeter.Graph.EraserRight ;

PeakOverloadLightX =   ob_VuMeter.Graph.PeakOverloadLightX ;
			PeakOverloadLightY =   ob_VuMeter.Graph.PeakOverloadLightY ;


AmplitudeLeft = ob_VuMeter.m_MeanValueLeft ;
AmplitudeRight = ob_VuMeter.m_MeanValueRight ;


tmRefresh.Enabled = true ;
			tmRefreshText.Enabled = true ;
		}

		private void tmRefresh_Tick(object sender, System.EventArgs e)
		{

// paint form
			System.Drawing.Graphics objGraphics;
			objGraphics = this.CreateGraphics();		

			Pen PenHigh  = new Pen(Color.Red );
			PenHigh.Width = LineWidth ;

			Pen PenNormal = new Pen(Color.Green);
			PenNormal.Width = LineWidth ;

			Pen PenLow = new Pen(Color.Yellow );
			PenLow.Width = LineWidth ;

			Pen PenVackground = new Pen(Color.White);
			PenVackground.Width = LineWidth ;

			objGraphics.DrawLine(PenHigh, LeftGraphX, HighTop , LeftGraphX, HighBottom);		
			objGraphics.DrawLine(PenHigh, RightGraphX, HighTop , RightGraphX, HighBottom);		

			objGraphics.DrawLine(PenNormal, LeftGraphX, NormalTop, LeftGraphX, NormalBottom);				objGraphics.DrawLine(PenNormal, RightGraphX, NormalTop, RightGraphX, NormalBottom);	
	objGraphics.DrawLine(PenLow, LeftGraphX, LowTop, LeftGraphX, LowBottom);	
			objGraphics.DrawLine(PenLow, RightGraphX, LowTop, RightGraphX, LowBottom);	
	
// Erase the unwanted line starting from top according to amplitude of each channel
			objGraphics.DrawLine(PenVackground , LeftGraphX, HighTop , LeftGraphX, EraserLeft );	
			objGraphics.DrawLine(PenVackground , RightGraphX, HighTop , RightGraphX, EraserRight );	
				
			// paint the peak overload light
			
			if ( BeepEnabled == false)
			{
				objGraphics.DrawLine(PenVackground , PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX , PeakOverloadLightY + LineWidth + LineWidth);	
				objGraphics.DrawLine(PenVackground , PeakOverloadLightX + LineWidth, PeakOverloadLightY, PeakOverloadLightX + LineWidth , PeakOverloadLightY + LineWidth + LineWidth);	
			}
			else  // Paint the light red for warning
			{
				objGraphics.DrawLine(PenHigh, PeakOverloadLightX+ LineWidth , PeakOverloadLightY, PeakOverloadLightX , PeakOverloadLightY + LineWidth + LineWidth);	
				objGraphics.DrawLine(PenHigh, PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX + LineWidth , PeakOverloadLightY + LineWidth + LineWidth);	
			}
			
		}
		
bool BeepEnabled = false ;
// catch the peak overload event triggered by VuMeter
		public void CatchPeakOverloadEvent ( VuMeter ob_VuMeter , PeakOverload ob_PeakOverload )
		{

			if (ob_PeakOverload .Channel == 1)
			{
				txtOverloadLeft.Text	 = ob_VuMeter.m_MeanValueLeft.ToString () ;

			}
			
			if (ob_PeakOverload .Channel== 2)
			{
				txtOverloadRight.Text = ob_VuMeter.m_MeanValueRight.ToString ()  ;
			}

BeepEnabled =true  ;
			}	

// repeats the LoadBeep function to repeat beeps while there is overload
		private void tmBeep_Tick(object sender, System.EventArgs e)
		{
if (BeepEnabled == true)
			LoadBeep () ;

BeepEnabled = false ;
		}

// Updates the amplitude of channels at regular intervals
		private void tmRefreshText_Tick(object sender, System.EventArgs e)
		{
		txtAmplitudeLeft.Text = AmplitudeLeft.ToString () ;
txtAmplitudeRight.Text = AmplitudeRight.ToString () ;
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
		this.Close () ;
		}

// end of class
	}
}
