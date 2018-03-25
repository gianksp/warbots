"""Bot Operating System

An event driven Operating System for Bots.

The BotOS class below is the only class you will have to manipulate
to control the bot.

- Actions (Commands issued to the bot):
Can be shoot, add_force, add_torque

- Events (Messages received from the simulation):
Are prefixed with "on_"

Get Started
-----------

Prerequisites:
- Python 2.7 (https://www.python.org/downloads)
- Pip (https://pip.pypa.io/en/stable/installing)

Run:
- python bot.py -l {link_id}

"""


class BotOS:
    """BotOS Bot Operating System.

    This is the class where you, as a programmer will interpret the events
    received from your bot from the web simulator session and from where
    you will issue the commands to control it.
    """
    def on_cycle(self):
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

    This class handles the connection with the simulator
    and the message transfer actions.

    It is highly recommended that you do not modify this class
    unless you know exactly what you are doing. This class is a wrapper
    for the BotOS class to communicate.

    Please stick to the BotOS class and extend your code from there
    to add more functionalities.
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
            # 60 FPS config
            time.sleep(0.01667)
            self.bot.on_cycle()

    def connect(self, cred, host, cert):
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
        cognito = boto3.client('cognito-identity', region_name=region)
        identity = cognito.get_id(IdentityPoolId=idp)["IdentityId"]
        session = cognito.get_credentials_for_identity(IdentityId=identity)
        return session["Credentials"]

    def download_cert(self, url, cert):
        if not os.path.exists(cert):
            print("Downloading certificate...")
            urllib.urlretrieve(url, cert)

    def transmit(self, action):
        self.ws.publish("{}-upstream".format(self.link),  json.dumps(action), 1)

    def received(self, client, userdata, message):
        data = json.loads(message.payload)
        if "w" in data:
            self.bot.on_rotation_change(data)
        elif "type" in data:
            self.bot.on_radar(data)
        else:
            self.bot.on_position_change(data)

try:
    import boto3
    from AWSIoTPythonSDK.MQTTLib import AWSIoTMQTTClient
except ImportError:
    import pip
    import sys
    pip.main(['install', '--user', 'boto3', 'AWSIoTPythonSDK'])
    print '\033[92m'
    print 'Dependencies installed successfully'
    print u'\u2713 boto3'
    print u'\u2713 AWSIoTPythonSDK'
    print 'Please run again your python bot.py -l {link_id} command'
    print u'\033[0m'
    sys.exit(0)

import time
import argparse
import urllib
import os
import json
import uuid

# Read in command-line parameters, expect -l parameter
parser = argparse.ArgumentParser()
parser.add_argument("-l", "--link", action="store", required=True, dest="link", help="Your bot link")

# Run BotOS and Link
args = parser.parse_args()
bot = BotOS()
Link(args.link, bot)
