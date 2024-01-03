using CsClient.Utils;

namespace CsClientTests.Mocks
{
    /// <summary>
    /// Mock Environment to inject as dependency.
    /// Overrides the <see cref="cs_client.Utils.Environment.GetEntropy"/> function.
    /// <see cref="cs_client.Utils.Environment"/>
    /// </summary>
    public class MockEnvironment : Environment
    {
        public string Entropy { get; set; } = Constants.DefaultEntropy;
        override public string GetEntropy()
        {
            return Entropy;
        }
    }
}
