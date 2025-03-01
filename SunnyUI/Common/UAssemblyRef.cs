namespace Sunny.UI
{
    /// <summary>
    /// Provides constant definitions for assembly references.
    /// </summary>
    internal static class AssemblyRefEx
    {
        /// <summary>
        /// Assembly version number.
        /// </summary>
        internal const string Version = "4.0.0.0";

        /// <summary>
        /// Microsoft public key.
        /// </summary>
        internal const string MicrosoftPublicKey = "b03f5f7f11d50a3a";

        /// <summary>
        /// System.Design assembly reference.
        /// </summary>
        internal const string SystemDesign = "System.Design, Version=" + Version + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;

        /// <summary>
        /// System.Drawing.Design assembly reference.
        /// </summary>
        internal const string SystemDrawingDesign = "System.Drawing.Design, Version=" + Version + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;

        /// <summary>
        /// System.Drawing assembly reference.
        /// </summary>
        internal const string SystemDrawing = "System.Drawing, Version=" + Version + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;
    }
}
