using System.Collections.Generic;
using System.Windows.Forms;

namespace MusicBeePlugin.Extensions.Windows.Forms
{
    public static class ControlExtension
    {
        /// <summary>
        /// コントロールが持つすべての子コントロールを取得します。
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static IEnumerable<Control> GetAllControls(this Control control)
            => control.GetAllControls<Control>();

        /// <summary>
        /// コントロールが持つ、指定した型のすべての子コントロールを取得します。
        /// </summary>
        /// <typeparam name="TResult">Control型を継承した任意の型</typeparam>
        /// <param name="control"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> GetAllControls<TResult>(this Control control)
            where TResult : Control
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl.Controls.Count > 0)
                {
                    IEnumerable<TResult> childs = ctrl.GetAllControls<TResult>();
                    foreach (TResult child in childs)
                    {
                        yield return child;
                    }
                }
                else if (ctrl is TResult tctrl)
                {
                    yield return tctrl;
                }
            }
        }

        /// <summary>
        /// 親コントロールの世代数を返します。
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static int GetDepth(this Control control)
        {
            int depth = 0;

            if (control.Parent != null)
            {
                depth += control.Parent.GetDepth() + 1;
            }

            return depth;
        }
    }
}
