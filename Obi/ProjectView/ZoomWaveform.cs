﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.media.timing;
using AudioLib;

namespace Obi.ProjectView
{
    public partial class ZoomWaveform : UserControl
    {
        private ContentView m_ContentView = null;
        private Strip m_Strip;
        private  EmptyNode m_Node;
        private AudioBlock m_AudioBlock;
        private static ZoomWaveform m_ZoomwaveForm;
        private int initialWaveformWidth = 0;
        private float m_ZoomFactor = 0;
        private ZoomWaveform()
        {
            
            InitializeComponent();
            //this.Controls.Add(hScrollBar);
            this.Controls.Add(panelZooomWaveform);
       
        }
        public static ZoomWaveform ZoomPanel
        {
            get { return m_ZoomwaveForm; }   
        }
        public ZoomWaveform(ContentView contentView, Strip strip,EmptyNode node ):this    ()
        {
            m_ContentView = contentView;
            m_Strip = strip;
            m_Node = node;
            if (m_ContentView != null)
            {
                this.Width = m_ContentView.Width-22;
                this.Height = m_ContentView.Height-22;
                 btnClose.Location = new Point(btnClose.Location.X, this.Height - 25);
                btnNextPhrase.Location=new Point(btnNextPhrase.Location.X,this.Height-25);
                btnPreviousPhrase.Location=new Point(btnPreviousPhrase.Location.X,this.Height-25);
                btnReset.Location=new Point(btnReset.Location.X,this.Height-25);
                btnZoomIn.Location=new Point(btnZoomIn.Location.X,this.Height-25);
                btnZoomOut.Location=new Point(btnZoomOut.Location.X,this.Height-25);
                panelZooomWaveform.Width = this.Width - 30;
                panelZooomWaveform.Height = this.Height - 30;
                
            }
           //this.Width=m_ContentView.Width;
            if (m_Node is PhraseNode)
            {
               
                m_AudioBlock = new AudioBlock((PhraseNode)m_Node, m_Strip);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                m_ZoomFactor = zoomFactor;
             //   m_AudioBlock.Width = m_ContentView.Width;
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
              //  m_AudioBlock.SetWaveformForZoom(m_Node as PhraseNode,zoomFactor);
              //int a=  m_AudioBlock.ComputeWaveformDefaultWidth();
                m_AudioBlock.Waveform.Render();
            }
        }

        
       // public Waveform Waveformval { get { return waveform1; } }
        private void button1_Click(object sender, EventArgs e)
        {
          
          
        }
        public void WaveForm(PhraseNode stripIndex,Strip st)
        {
           
            //AudioBlock temp = new AudioBlock(stripIndex,st,waveform1);
            
        //    temp.Waveform = waveform1;
            // temp.Show();

            //Waveform1
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
         m_ContentView.RemovePanel();
        }

        private void btnNextPhrase_Click(object sender, EventArgs e)
        {
            ObiNode nextNode = m_Node.FollowingNode;
            
            if(nextNode is PhraseNode)
            {
                m_Node = nextNode as PhraseNode;
                if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                {
                    panelZooomWaveform.Controls.Remove(m_AudioBlock);
                }
                m_AudioBlock = new AudioBlock((PhraseNode)nextNode, m_Strip);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
               // float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                m_AudioBlock.SetZoomFactorAndHeight(m_ZoomFactor, Height);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
              //  m_AudioBlock.SetWaveformForZoom(m_Node as PhraseNode,zoomFactor);
                m_AudioBlock.Waveform.Render();
            }


        }

        private void btnPreviousPhrase_Click(object sender, EventArgs e)
        {
            ObiNode previousNode = m_Node.PrecedingNode;

            if (m_Node.PrecedingNode is PhraseNode)
            {
                m_Node = previousNode as PhraseNode;
                if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                {
                    panelZooomWaveform.Controls.Remove(m_AudioBlock);
                }
                m_AudioBlock = new AudioBlock((PhraseNode)previousNode, m_Strip);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
             //   float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                m_AudioBlock.SetZoomFactorAndHeight(m_ZoomFactor, panelZooomWaveform.Height);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
               // m_AudioBlock.SetWaveformForZoom(m_Node as PhraseNode,zoomFactor);
                m_AudioBlock.Waveform.Render();
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
;
            //float zoomFactor = this.Height / m_AudioBlock.Height;
            m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
        //  panelZooomWaveform.Height = panelZooomWaveform.Height + (int)(panelZooomWaveform.Height * 0.5);
         //   float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
           // m_AudioBlock.SetWaveformForZoom(m_Node as PhraseNode, zoomFactor);
            m_AudioBlock.Waveform.Render();

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
           // panelZooomWaveform.Height = panelZooomWaveform.Height - (int)(panelZooomWaveform.Height * 0.5);
          //  float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;

            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
            
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            
          //  m_AudioBlock.SetWaveformForZoom(m_Node as PhraseNode,m_Zoomfactor);
            m_AudioBlock.Waveform.Render();

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_AudioBlock.Waveform.Width = initialWaveformWidth;
            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);

            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Render();

        }

       // public Panel Panel_WaveForm { get { return panel_ZoomWaveform; } }
    }
}