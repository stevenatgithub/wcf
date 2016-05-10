// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Xunit;

public static partial class XmlSerializerFormatTests
{
    [Fact]
    [OuterLoop]
    public static void MessageHeader_ResponseTypeWithUsesMessageHeaderAttribute()
    {
        // *** SETUP *** \\
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic);
        var factory = new ChannelFactory<IXmlMessageContarctTestService>(binding, endpointAddress);
        IXmlMessageContarctTestService serviceProxy = factory.CreateChannel();
        var input = new XmlMessageContractTestRequest("1");

        try
        {
            // *** EXECUTE *** \\
            XmlMessageContractTestResponse response = serviceProxy.EchoMessageResponseWithMessageHeader(input);

            // *** VALIDATE *** \\
            Assert.NotNull(response);
            Assert.Equal(input.Message, response.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [Fact]
    [OuterLoop]
    [ActiveIssue(702)]
    public static void MessageHeader_RequestTypeWithUsesMessageHeaderAttribute()
    {
        // *** SETUP *** \\
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic);
        var factory = new ChannelFactory<IXmlMessageContarctTestService>(binding, endpointAddress);
        IXmlMessageContarctTestService serviceProxy = factory.CreateChannel();
        var input = new XmlMessageContractTestRequestWithMessageHeader("1");

        try
        {
            // *** EXECUTE *** \\
            XmlMessageContractTestResponse response = serviceProxy.EchoMessageResquestWithMessageHeader(input);

            // *** VALIDATE *** \\
            Assert.NotNull(response);
            Assert.Equal(input.Message, response.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [Fact]
    [OuterLoop]
    public static void OperationContextScope_HttpRequestCustomMessageHeader_RoundTrip_Verify()
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        MyClientBase<IWcfServiceXml_OperationContext> client = new MyClientBase<IWcfServiceXml_OperationContext>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfServiceXml_OperationContext serviceProxy = client.ChannelFactory.CreateChannel();

        string customHeaderName = "TestSessionHeader";
        string customHeaderNS = "xmlns=urn:TestWebServices/MyWebService/";
        var customHeaderValue = new MesssageHeaderCreateHeaderWithXmlSerializerTestType { Message = "secret" };

        try
        {
            using (OperationContextScope scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                // *** EXECUTE *** \\
                MessageHeader header
                  = MessageHeader.CreateHeader(
                  customHeaderName,
                  customHeaderNS,
                  customHeaderValue
                  );
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                string result = serviceProxy.GetIncomingMessageHeadersMessage(customHeaderName, customHeaderNS);

                // *** VALIDATE *** \\
                Assert.Equal(customHeaderValue.Message, result);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client);
        }
    }

}
