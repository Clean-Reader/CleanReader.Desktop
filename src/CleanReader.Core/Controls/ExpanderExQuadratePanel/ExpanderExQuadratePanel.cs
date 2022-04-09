// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.Core
{
    /// <summary>
    /// Simulates the UI of Expander,
    /// but does not provide expand/collapse functions,
    /// and is used as a display container when <see cref="ExpanderEx.Content"/> is empty.
    /// </summary>
    public partial class ExpanderExQuadratePanel : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderExQuadratePanel"/> class.
        /// </summary>
        public ExpanderExQuadratePanel()
        {
            this.DefaultStyleKey = typeof(ExpanderExQuadratePanel);
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer() => new ExpanderExQuadratePanelAutomationPeer(this);
    }
}
