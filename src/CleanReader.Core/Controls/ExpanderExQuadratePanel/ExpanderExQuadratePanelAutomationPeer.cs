// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml.Automation.Peers;

namespace CleanReader.Core
{
    /// <summary>
    /// Exposes <see cref="ExpanderExQuadratePanel"/> to Microsoft UI Automation.
    /// </summary>
    public class ExpanderExQuadratePanelAutomationPeer : ButtonAutomationPeer
    {
        private readonly ExpanderExQuadratePanel owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderExQuadratePanelAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner element to create for.</param>
        public ExpanderExQuadratePanelAutomationPeer(ExpanderExQuadratePanel owner)
            : base(owner) => this.owner = owner;

        /// <inheritdoc/>
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;
    }
}
