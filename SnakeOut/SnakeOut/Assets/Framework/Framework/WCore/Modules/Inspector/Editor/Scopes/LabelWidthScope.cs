using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
    public class LabelWidthScope : GUI.Scope
    {
        private readonly float defaultWidth;

        public LabelWidthScope(float width)
        {
            defaultWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = width;
        }

        protected override void CloseScope()
        {
            EditorGUIUtility.labelWidth = defaultWidth;
        }
    }
}