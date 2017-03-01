using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageSorter
{
    class ImageSorterApplicationContext : ApplicationContext
    {

        private int windowCount = 0;

        private static ImageSorterApplicationContext context;

        private ImageSorterApplicationContext()
        {

        }

        public static ImageSorterApplicationContext GetContext()
        {
            if(context == null)
            {
                context = new ImageSorterApplicationContext();
            }
            return context;
        }

        public void RunNew()
        {
            MainForm form = new MainForm();
            new Controller(form);

            windowCount++;

            form.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            form.Show();
        }
    }
}
