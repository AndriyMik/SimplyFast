﻿using System;

namespace SimplyFast.Log.Internal.Loggers
{
    public class ChildLogger : Logger
    {
        private readonly ILogger _root;
        private bool _severityOverriden;

        public ChildLogger(ILogger root, string name): base(name)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            _root = root;
        }

        public override Severity Severity
        {
            get { return _severityOverriden ? base.Severity : _root.Severity; }
            set
            {
                _severityOverriden = true;
                base.Severity = value;
            }
        }

        protected override void DoLog(IMessage message)
        {
            _root.Log(message);
            base.DoLog(message);
        }
    }
}