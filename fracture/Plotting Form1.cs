using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Toolboxes;
namespace fracture
{
    public partial class Plotting_Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        public Plotting_Form1()
        {
            InitializeComponent();
          //  ribbonControl1.SelectedPage = ribbonControl1.PageCategories[0].Pages[0];
          
        }


  
            private void ilPanel1_Load(object sender, EventArgs e) {
            // create a simple plot with a colorbar
            ilPanel1.Scene.Add(new ILPlotCube() {
                new ILSurface(ILSpecialData.sincf(50,40)) {
                    new ILColorbar()
                }
            });
            // get the colorbar for configuration
            var cb = ilPanel1.SceneSyncRoot.First<ILColorbar>();  // <- use SceneSyncRoot before 4.3! (see below)
            cb.Background.Color = Color.FromArgb(230,230,255);
            cb.Scale(new Vector3(0.1, 0.1, 1));           // configure a custom tick creation function
            cb.First<ILTickCollection>().TickCreationFuncEx = MyTicksCreationFunc;

            // Note: until ILNumerics Ultimate VS vers. 4.3 the setting for ILTickCollection.TickCreationFuncEx 
            // has not been synchronized with the synchronized copy used for rendering yet. Thererfore, in order 
            // to configure a custom TickCreationFuncEx, one must assign the custom function to the 
            // tick collection in the synchronized tree in these versions. For example:
            // ilPanel1.SceneSyncRoot.First<ILColorbar>().First<ILTickCollection>().TickCreationFuncEx = MyTicksCreationFunc
            //
            // Up from version 4.3 the same setting for the TickCreationFuncEx is used in the synchronized 
            // scene as in the global scene. Hence the configuration can happen directly on the glbal scene. 

        }
        /// <summary>
        /// This function gets called by ILTickCollection on every attempt to build/render the ticks for an axis. Here
        /// it is called from the colorbar rendering code. Implementors of this function can provide their own tick collections.
        /// Often such custom ticks are based on the standard implementation provided by ILTickCollection.CreateTicksAuto().
        /// </summary>
        /// <param name="min">min value for the axis range</param>
        /// <param name="max">max value for the axis range</param>
        /// <param name="numberTicks">rough estimate of how many ticks will fit inside the availabel space for the the whole axis scale</param>
        /// <param name="axis">a reference to the axis referenced by the tick collection</param>
        /// <param name="scale">type of scaling of the axis. For ILColorbar: linear only</param>
        /// <returns>Your own collection of ticks for rendering</returns>
        IEnumerable<ILTick> MyTicksCreationFunc(float min, float max, int numberTicks, ILAxis axis, AxisScale scale = AxisScale.Linear) {
            // a custom tick creating function: use the standard ticks collection and add custom ticks for min and max values
            return ILTickCollection.CreateTicksAuto(min, max, numberTicks, axis, scale)
                .Concat(new [] { createTick(min), createTick(max) });
        }

        /// <summary>
        /// helper function for creating custom ticks with preconfigured labels
        /// </summary>
        /// <param name="val">position of the tick, also used for label text creation</param>
        /// <returns>new tick</returns>
        private ILTick createTick(float val) {
            var ret = new ILTick(val, new ILLabel(val.ToString("F2")) { 
                // right align the tick label to the tick lines
                Anchor = new PointF(1.2f,.5f),
                Color = Color.Red,
                Font = new Font("微软雅黑",9)
            });
            // disable auto updating of the label text. If this is true, LabelCreationFunc is used and overwrites the custom value!
            ret.AutoLabel = false;

            return ret; 
        }

       
    }
}
