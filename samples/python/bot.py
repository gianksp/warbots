"""Bot Operating System

An event driven Operating System for Bots

Variables:
	parser {[type]} -- [description]
	parser.add_argument("-l", "--link", action {str} -- [description]
	args {[type]} -- [description]
	bot {[type]} -- [description]
	Link(args.link, bot) {[type]} -- [description]
"""
import boto3
from AWSIoTPythonSDK.MQTTLib import AWSIoTMQTTClient
import time
import argparse
import urllib
import os
import json
import uuid


class BotOS:
    """BotOS Bot Operating System.
    
    Control the actions of a Bot via link
    """
    def cycle(self):
    	"""Cycle event.
    	
    	Method that gets called on every cycle. Currently set to be called
    	on every frame (60 frames per second configuration)
    	"""
        print("Your code for handling each cycle here")

    def on_radar(self, obj):
    	"""Object in radar event.
    	
    	Method that gets called when the radar sensor on the bot detects
    	obstacles, items, allies or enemies on radar
    	
    	Arguments:
    		obj {[type]} -- [description]
    	"""
        print("Your code for handling radar detection here")

    def on_position_change(self, position):
    	"""Bot position change event.
    	
    	Method that gets called when the position of the bot changes
    	
    	Arguments:
    		position {[type]} -- [description]
    	"""
        print("Your code for handling position change here")

    def on_rotation_change(self, rotation):
    	"""Bot rotation change event.
    	
    	Method that gets called when the rotation of the bot changes
    	
    	Arguments:
    		rotation {[type]} -- [description]
    	"""
        print("Your code for handling rotation change here")


class BotAction:
    """Actions that can be transmitted to the bot.
    
    Supported bot actions
    - shoot (toggle True, False)
    - add_torque rotation (Vector x, y, z)
    - add_force translation (Vector x, y, z)
    """
    def shoot(active):
    	"""Toggle on shooting.
    	
    	Shoot while True
    	
    	Arguments:
    		active {Boolean} -- Toggle shooting
    	"""
        Link.transmit({"fire": active})

    def add_torque(x, y, z, w):
    	"""Add torque for rotation.
    	
    	Add a torque force over the bot
    	
    	Arguments:
    		x {[type]} -- [description]
    		y {[type]} -- [description]
    		z {[type]} -- [description]
    		w {[type]} -- [description]
    	"""
        Link.transmit({"torque": {"x": x,"y": y,"z": z}})

    def add_force(x, y, z):
    	"""Add force for translation.
    	
    	Add a moving force
    	
    	Arguments:
    		x {[type]} -- [description]
    		y {[type]} -- [description]
    		z {[type]} -- [description]
    	"""
        Link.transmit({"force": {"x": x,"y": y,"z": z}})

class Link:
    """Establish link between web simulator and this worker.
    
    Link
    """
    def __init__(self, link, bot):
        self.bot = bot
        self.link = link
        # Meta
        cert_url = 'https://www.symantec.com/content/en/us/enterprise/verisign/roots/VeriSign-Class%203-Public-Primary-Certification-Authority-G5.pem'
        cert_path = 'cert.pem'
        idp = 'us-east-1:e0f2dcf7-3b7a-4151-ac45-da517964bd2b'
        host = 'a1pfvzfzeiz9sa.iot.us-east-1.amazonaws.com'
        region = host.split('.')[2]
        # Download cert and get cognito session
        self.download_cert(cert_url, cert_path)
        credentials = self.get_session(idp, region)
        # Connect to iot with cognito session
        self.ws = self.connect(credentials, host, cert_path)
        # Subscribe to messages from simulator to this worker
        self.ws.subscribe("{}-downstream".format(self.link), 1, self.received)
        while True:
            time.sleep(0.01667)
            self.bot.cycle()

    def connect(self, cred, host, cert):
    	"""Connect to IoT
    	
    	[description]
    	
    	Arguments:
    		cred {[type]} -- [description]
    		host {[type]} -- [description]
    		cert {[type]} -- [description]
    	
    	Returns:
    		[type] -- [description]
    	"""
        ws = AWSIoTMQTTClient(uuid.uuid4().hex, useWebsocket=True)
        ws.configureEndpoint(host, 443)
        ws.configureCredentials(cert)
        ws.configureIAMCredentials(cred["AccessKeyId"], cred["SecretKey"], cred["SessionToken"])
        ws.configureAutoReconnectBackoffTime(1, 32, 20)
        ws.configureOfflinePublishQueueing(-1)
        ws.configureDrainingFrequency(2)
        ws.configureConnectDisconnectTimeout(10)
        ws.configureMQTTOperationTimeout(5)
        ws.connect()
        return ws

    def get_session(self, idp, region):
    	"""Get Cognito session credentials
    	
    	[description]
    	
    	Arguments:
    		idp {[type]} -- [description]
    		region {[type]} -- [description]
    	
    	Returns:
    		[type] -- [description]
    	"""
        cognito = boto3.client('cognito-identity', region_name=region)
        identity = cognito.get_id(IdentityPoolId=idp)["IdentityId"]
        session = cognito.get_credentials_for_identity(IdentityId=identity)
        return session["Credentials"]

    def download_cert(self, url, cert):
    	"""Download certificate
    	
    	[description]
    	
    	Arguments:
    		url {[type]} -- [description]
    		cert {[type]} -- [description]
    	"""
        if not os.path.exists(cert):
            print("Downloading certificate...")
            urllib.urlretrieve(url, cert)

    def transmit(self, action):
    	"""Send messages upstream to web simulator
    	
    	[description]
    	
    	Arguments:
    		action {[type]} -- [description]
    	"""
        self.ws.publish("{}-upstream".format(self.link),  json.dumps(action), 1)

    def received(self, client, userdata, message):
    	"""Receive messages downstream from simulator
    	
    	[description]
    	
    	Arguments:
    		client {[type]} -- [description]
    		userdata {[type]} -- [description]
    		message {[type]} -- [description]
    	"""
        data = json.loads(message.payload)
        if "w" in data:
            self.bot.on_rotation_change(data)
        elif "type" in data:
            self.bot.on_radar(data)
        else:
            self.bot.on_position_change(data)

# Read in command-line parameters, expect -l parameter
parser = argparse.ArgumentParser()
parser.add_argument("-l", "--link", action="store", required=True, dest="link", help="Your bot link")

# Run BotOS and Link
args = parser.parse_args()
bot = BotOS()
Link(args.link, bot)
