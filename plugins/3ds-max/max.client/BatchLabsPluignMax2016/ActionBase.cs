
using System;
using System.Windows.Forms;

using UiViewModels.Actions;

namespace BatchLabsRendering
{
    /// <inheritdoc />
    /// <summary>
    /// Base class to avoid having to declare ActionText and Category in more than one location.
    /// </summary>
    public abstract class ActionBase : CuiActionCommandAdapter
    {
        protected ActionBase()
        {
            LabsRequestHandler = new BatchLabsRequestHandler();
        }

        public BatchLabsRequestHandler LabsRequestHandler { get; }

        public override string ActionText => InternalActionText;

        public override string Category => InternalCategory;

        public override string InternalCategory => "BatchLabs - Rendering";

        /// <summary>
        /// Execute is called when the user clicks on the action in the UI.
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            try
            {
                InternalExecute();
            }
            catch (Exception e)
            {
                MessageBox.Show("Uncaught exception occurred: " + e.Message);
            }
        }

        public abstract void InternalExecute();
    }
}
