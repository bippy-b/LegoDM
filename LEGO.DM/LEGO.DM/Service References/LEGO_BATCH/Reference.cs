﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5420
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LEGO.DM.LEGO_BATCH {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://services.model.irs.lego.icon.com/", ConfigurationName="LEGO_BATCH.ScheduledTaskRunner")]
    public interface ScheduledTaskRunner {
        
        // CODEGEN: Generating message contract since element name arg0 from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        LEGO.DM.LEGO_BATCH.runDMResponse runDM(LEGO.DM.LEGO_BATCH.runDMRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class runDMRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="runDM", Namespace="http://services.model.irs.lego.icon.com/", Order=0)]
        public LEGO.DM.LEGO_BATCH.runDMRequestBody Body;
        
        public runDMRequest() {
        }
        
        public runDMRequest(LEGO.DM.LEGO_BATCH.runDMRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class runDMRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string arg0;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int arg1;
        
        public runDMRequestBody() {
        }
        
        public runDMRequestBody(string arg0, int arg1) {
            this.arg0 = arg0;
            this.arg1 = arg1;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class runDMResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="runDMResponse", Namespace="http://services.model.irs.lego.icon.com/", Order=0)]
        public LEGO.DM.LEGO_BATCH.runDMResponseBody Body;
        
        public runDMResponse() {
        }
        
        public runDMResponse(LEGO.DM.LEGO_BATCH.runDMResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class runDMResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string @return;
        
        public runDMResponseBody() {
        }
        
        public runDMResponseBody(string @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface ScheduledTaskRunnerChannel : LEGO.DM.LEGO_BATCH.ScheduledTaskRunner, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ScheduledTaskRunnerClient : System.ServiceModel.ClientBase<LEGO.DM.LEGO_BATCH.ScheduledTaskRunner>, LEGO.DM.LEGO_BATCH.ScheduledTaskRunner {
        
        public ScheduledTaskRunnerClient() {
        }
        
        public ScheduledTaskRunnerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ScheduledTaskRunnerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ScheduledTaskRunnerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ScheduledTaskRunnerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LEGO.DM.LEGO_BATCH.runDMResponse LEGO.DM.LEGO_BATCH.ScheduledTaskRunner.runDM(LEGO.DM.LEGO_BATCH.runDMRequest request) {
            return base.Channel.runDM(request);
        }
        
        public string runDM(string arg0, int arg1) {
            LEGO.DM.LEGO_BATCH.runDMRequest inValue = new LEGO.DM.LEGO_BATCH.runDMRequest();
            inValue.Body = new LEGO.DM.LEGO_BATCH.runDMRequestBody();
            inValue.Body.arg0 = arg0;
            inValue.Body.arg1 = arg1;
            LEGO.DM.LEGO_BATCH.runDMResponse retVal = ((LEGO.DM.LEGO_BATCH.ScheduledTaskRunner)(this)).runDM(inValue);
            return retVal.Body.@return;
        }
    }
}
