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
			self.linkz.emit(msg);
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

	copyClipboard () {

		var textField = document.createElement('textarea');
    textField.innerText = document.getElementById('link_id').innerText;
    document.body.appendChild(textField);
    textField.select();
    document.execCommand('copy');
    textField.remove();
	}

	transmitMessage(message) {
		// Send to client listening
		this.mqttClient.publish(this.state.link+'-downstream', message.toString());
	  }

	render() {

		var status = this.state.connected ? (
			<div>
	      		<div>Connected!</div>
	      		<div>Download <a href='https://cdn.rawgit.com/gianksp/warbots/master/samples/python/bot.py'>bot.py</a> and run: <b>python bot.py -l {this.state.link}</b> </div>
	      	</div>
	    ) : (
	      <div>Connecting...</div>
	    );

		var lnk = (<a id="link_id" onClick={this.copyClipboard}>{this.state.link}</a>);

		return (
			<div>
			    <div className="columns">
			        <div className="column is-half is-offset-one-quarter">
			            <div>Bot link: {lnk}</div>
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
