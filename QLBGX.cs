using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLBGX
{
    public partial class QLBGX : Form
    {
        QLRaVao qLRaVao;
        QLX qlX;
        public QLBGX()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
        }

        private void btnQLRaVao_Click(object sender, EventArgs e)
        {
            if (qLRaVao == null)
            {
                qLRaVao = new QLRaVao();
                qLRaVao.MdiParent = this;
                qLRaVao.FormClosed += QLRaVao_FormClosed;
                qLRaVao.Show();
                qLRaVao.Dock = DockStyle.Fill;
                //this.Hide(); bỏ comment để ẩn form cha khi mở form con

            }
            else
            {
                qLRaVao.Activate();
                qLRaVao.BringToFront();
            }
        }
        private void QLRaVao_FormClosed(object sender, FormClosedEventArgs e)
        {
            qLRaVao = null;
            
        }

        private void btnQLX_Click(object sender, EventArgs e)
        {
            if (qlX == null)
            {
                qlX = new QLX();
                qlX.MdiParent = this;
                qlX.FormClosed += QlX_FormClosed;
                qlX.Show();
                qlX.Dock = DockStyle.Fill;
                //this.Hide(); bỏ comment để ẩn form cha khi mở form con
            }
            else
            {
                qlX.Activate();
                qlX.BringToFront();
            }
        }
        private void QlX_FormClosed(object sender, FormClosedEventArgs e)
        {
            qlX = null;
        }
    }
}
