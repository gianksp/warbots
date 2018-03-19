import React from 'react'

import Shortid from 'shortid'
import AWS from 'aws-sdk'
import AWSIoTData from 'aws-iot-device-sdk'
import AWSConfiguration from '../../aws-configuration.js'

import Unity from "react-unity-webgl";
import { UnityEvent } from "react-unity-webgl";
import { RegisterExternalListener }  from "react-unity-webgl";

var msg = 1;

export class Home extends React.Component {

	connect() {
		var self = this;
		console.log('Loaded AWS SDK for JavaScript and AWS IoT SDK for Node.js');
		// var topic = 'subscribe-topic';

		var clientId = 'mqtt-explorer-' + (Math.floor((Math.random() * 100000) + 1));

		AWS.config.region = AWSConfiguration.region;
		AWS.config.credentials = new AWS.CognitoIdentityCredentials({
		   IdentityPoolId: AWSConfiguration.poolId
		});

		var mqttClient = AWSIoTData.device({
		   region: AWS.config.region,
		   host:AWSConfiguration.host,
		   clientId: clientId,
		   protocol: 'wss',
		   maximumReconnectTimeMs: 8000,
		   debug: true,
		   accessKeyId: '',
		   secretKey: '',
		   sessionToken: ''
		});

		var cognitoIdentity = new AWS.CognitoIdentity();
		AWS.config.credentials.get(function(err, data) {
			console.log("so what");
		   if (!err) {
		      console.log('retrieved identity: ' + AWS.config.credentials.identityId);
		      var params = {
		         IdentityId: AWS.config.credentials.identityId
		      };
		      cognitoIdentity.getCredentialsForIdentity(params, function(err, data) {
		         if (!err) {
		            console.log(data);
		            console.log('------------------');
		            mqttClient.updateWebSocketCredentials(data.Credentials.AccessKeyId,
		               data.Credentials.SecretKey,
		               data.Credentials.SessionToken);
		         } else {
		            console.log('error retrieving credentials: ' + err);
		            alert('error retrieving credentials: ' + err);
		         }
		      });
		   } else {
		      console.log('error retrieving identity:' + err);
		      alert('error retrieving identity: ' + err);
		   }
		});

		mqttClient.on('connect', function() {
			
			// console.log(lnk);
			self.setState({ connected: true });
			mqttClient.subscribe(self.state.link+'-upstream');
		});

		mqttClient.on('offline', function() {
			
			// console.log(lnk);
			self.setState({ connected: false });
		});

		// mqttClient.on('reconnect', window.mqttClientReconnectHandler);
		mqttClient.on('message', function(topic, payload) {
			var msg = payload.toString();
			// var send = msg.toInt()+1;
		
			self.setState({ messages: self.state.messages+'\n'+msg });
			console.log('message: ' + topic + ':' + msg);
			// mqttClient.publish(self.state.link+'-downstream', msg.toString());
			if (self.linkz.canEmit() === true) {
				console.log("transmiting to simulator....");
				self.linkz.emit(msg);
			}
		});

		return mqttClient;
	}

	componentDidMount() {
	  
		this.setState({ linkt: 'connecting...' });
	}

	constructor() {
		super();
		
		var self = this;
		setTimeout(function(){
			self.linkz = new UnityEvent("Link", "MessageReceived");
			RegisterExternalListener("TransmitMessage", self.transmitMessage.bind(self));
			console.log("--------INITIALISED-----------");
			// if (self.linkz.canEmit() === true) {
			// 	console.log("CAN EMIT!!");
			// 	self.linkz.emit("hello");
			// }
		},10000);
		// RegisterExternalListener("TransmitMessage", this.transmitMessage.bind(this));
		console.log("Constructing");
		var lnk = Shortid.generate();
		this.state = { link: lnk, connected: false, messages:'' };
		this.mqttClient = this.connect();
	}

	onProgress (progression) {
	  console.log ('Loading '+(progression * 100)+'%')
	  if (progression === 1) {
	  	console.log("--->LOADING DONE")
	  }
	}

	transmitMessage(message) {
		// Send to client listening
		
		this.mqttClient.publish(this.state.link+'-downstream', message.toString());

		// console.log(message);
	 //    if (this.linkz.canEmit() === true) {
		// 		console.log("2 transmiting to simulator.... "+message);
		// 		var yo = this;
		// 		setTimeout(function(){
		// 			yo.linkz.emit(message);
		// 		}, 250);
		// 	}
	  }

	render() {

		var status = this.state.connected ? (
			<div>
	      		<div>Connected!</div>
	      		<div>Download <a href='https://cdn.rawgit.com/gianksp/warbots/master/samples/python/bot.py'>bot.py</a> and run: python LINK={this.state.link} bot.py</div>
	      	</div>
	    ) : (
	      <div>Connecting...</div>
	    );

		return (
			<div>
			    <div className="columns">
			        <div className="column is-half is-offset-one-quarter">
			            <div>Bot link: {this.state.link} </div>
			            <div>{status}</div>
			            <div>{this.state.messages}</div>
			        </div>
			    </div>
			    <div className="all">
			    	<Unity onProgress={ this.onProgress } src="Build/sim.json" loader="Build/UnityLoader.js"/>
			    </div>
			</div>
		)
   	}
}