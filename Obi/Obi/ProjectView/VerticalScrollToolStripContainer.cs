using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class VerticalScrollToolStripContainer : UserControl
    {
        private ContentView m_ContentView = null;
        private bool m_CanScrollUp = true;
        private bool m_CanScrollDown = true;
        private int m_OriginalPanelSize;        
               
        public VerticalScrollToolStripContainer()
        {
            InitializeComponent();
            TrackBarValueInPercentage = 0;
            m_OriginalPanelSize = this.Height;                     
        }
        public ContentView contentView
        {
            get { return m_ContentView; }
            set { m_ContentView = value; }
        }

        public int TrackBarValueInPercentage
        {
            get { return 100 - trackBar1.Value; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    trackBar1.Value = 100 - value;
                }
            }
        }

        public bool CanScrollUp
        {
            get { return m_CanScrollUp; }
            set
            {
                m_CanScrollUp = value;
                m_BtnGoToBegining.Enabled =
                    m_BtnLargeIncrementUp.Enabled =
                    m_BtnSmallIncrementUp.Enabled = m_CanScrollUp;

                if (!m_CanScrollUp) TrackBarValueInPercentage = 0;
            }
        }

        public bool CanScrollDown
        {
            get { return m_CanScrollDown; }
            set
            {
                m_CanScrollDown = value;
                m_BtnGoToEnd.Enabled =
                m_BtnLargeIncrementDown.Enabled =
                 m_BtnSmallIncrementDown.Enabled = m_CanScrollDown;

                if (!m_CanScrollDown) TrackBarValueInPercentage = 100;
            }
        }

       

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackBar1.Capture = false;
        }

        public void UpdateScrollButtons(bool value)
        {
            m_BtnGoToBegining.Enabled = value && m_CanScrollUp;
            m_BtnLargeIncrementUp.Enabled = value && m_CanScrollUp;
            m_BtnSmallIncrementUp.Enabled = value && m_CanScrollUp;

            m_BtnGoToEnd.Enabled = value && m_CanScrollDown;
            m_BtnLargeIncrementDown.Enabled = value && m_CanScrollDown;
            m_BtnSmallIncrementDown.Enabled = value && m_CanScrollDown;

            //Console.WriteLine ( "down button status : " + m_BtnGoToEnd.Enabled + " Scroll up button status : " + m_BtnGoToBegining.Enabled );
        }

        private void m_BtnGoToBegining_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollStripsPanel_Top();
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnLargeIncrementUp_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollUp_LargeIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementUp_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollUp_SmallIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollDown_SmallIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnLargeIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollDown_LargeIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnGoToEnd_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollStripsPanel_Bottom();
                UpdateScrollButtons(true);
            }
        }

        public void verticalScrolling()
        {
            if ((m_OriginalPanelSize - this.Height) > 20)
            {
                m_BtnGoToBegining.Size = new Size(m_BtnGoToBegining.Width, 30);
                m_BtnGoToEnd.Size = new Size(m_BtnGoToEnd.Width, 30);
                m_BtnLargeIncrementDown.Size = new Size(m_BtnLargeIncrementDown.Width, 30);
                m_BtnLargeIncrementUp.Size = new Size(m_BtnLargeIncrementUp.Width, 30);
                m_BtnSmallIncrementDown.Size = new Size(m_BtnSmallIncrementDown.Width, 30);
                m_BtnSmallIncrementUp.Size = new Size(m_BtnSmallIncrementDown.Width, 30);
                toolStripTop.Size = new Size(toolStripBottom.Size.Width, 137);
                toolStripBottom.Size = new Size(toolStripTop.Size.Width, 137);
                trackBar1.Location = new Point(trackBar1.Location.X, toolStripTop.Location.Y + toolStripTop.Height);
                toolStripBottom.Location = new Point(toolStripBottom.Location.X, (Convert.ToInt32(trackBar1.Location.Y + trackBar1.Height)));
                toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, (toolStripTop.Height + toolStripBottom.Height + trackBar1.Height) + 180);                 
            }
            if((this.Height > m_OriginalPanelSize - 20) && (this.Height < m_OriginalPanelSize + 20))
            {
                m_BtnGoToBegining.Size = new Size(m_BtnGoToBegining.Width, 44);
                m_BtnGoToEnd.Size = new Size(m_BtnGoToEnd.Width, 44);
                m_BtnLargeIncrementDown.Size = new Size(m_BtnLargeIncrementDown.Width, 44);
                m_BtnLargeIncrementUp.Size = new Size(m_BtnLargeIncrementUp.Width, 44);
                m_BtnSmallIncrementDown.Size = new Size(m_BtnSmallIncrementDown.Width, 44);
                m_BtnSmallIncrementUp.Size = new Size(m_BtnSmallIncrementDown.Width, 44);
                toolStripTop.Size = new Size(toolStripBottom.Size.Width, 195);
                toolStripBottom.Size = new Size(toolStripTop.Size.Width, 195);
                trackBar1.Location = new Point(trackBar1.Location.X, toolStripTop.Location.Y + toolStripTop.Height);
                toolStripBottom.Location = new Point(toolStripBottom.Location.X, (Convert.ToInt32(trackBar1.Location.Y + trackBar1.Height)));
                toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, (toolStripTop.Height + toolStripBottom.Height + trackBar1.Height) + 180);                 
            }
            if ((this.Height - m_OriginalPanelSize) > 20)
            {
                m_BtnGoToBegining.Size = new Size(m_BtnGoToBegining.Width, 52);
                m_BtnGoToEnd.Size = new Size(m_BtnGoToEnd.Width, 52);
                m_BtnLargeIncrementDown.Size = new Size(m_BtnLargeIncrementDown.Width, 52);
                m_BtnLargeIncrementUp.Size = new Size(m_BtnLargeIncrementUp.Width, 52);
                m_BtnSmallIncrementDown.Size = new Size(m_BtnSmallIncrementDown.Width, 52);
                m_BtnSmallIncrementUp.Size = new Size(m_BtnSmallIncrementDown.Width, 52);
                toolStripTop.Size = new Size(toolStripBottom.Size.Width, 234);
                toolStripBottom.Size = new Size(toolStripTop.Size.Width, 234);
                trackBar1.Location = new Point(trackBar1.Location.X, toolStripTop.Location.Y + toolStripTop.Height);
                toolStripBottom.Location = new Point(toolStripBottom.Location.X, (Convert.ToInt32(trackBar1.Location.Y + trackBar1.Height)));
                toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, (toolStripTop.Height + toolStripBottom.Height + trackBar1.Height) + 180);                 
            }
        }

        private void VerticalScrollToolStripContainer_Resize(object sender, EventArgs e)
        {
            verticalScrolling();        
        }       
    }
}