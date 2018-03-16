using System.Collections; 
using System.Collections.Generic; 
using System; 
using UnityEngine; 
using UnityEngine.UI;

using Apache.NMS; 
using Apache.NMS.Util; 
using Apache.NMS.ActiveMQ; 
using System.Threading;

public class AMQController : MonoBehaviour {

	private IConnectionFactory __factory;
	private IConnection __connection;
	private ISession __session;
	private IDestination __destinationBot;
	private IDestination __destinationSim;
	private IMessageConsumer __consumer;
	private IMessageProducer __producer;

	private Bot __bot;


	// Use this for initialization
	void Start () 
	{

		__bot = gameObject.GetComponent<Bot> ();

		__factory = new ConnectionFactory("tcp://localhost:61616/");
		__connection = __factory.CreateConnection("admin", "admin");
		__connection.Start();
		__session = __connection.CreateSession();

		__destinationBot = SessionUtil.GetDestination(__session, "queue://warbots.bot");
		__destinationSim = SessionUtil.GetDestination(__session, "queue://warbots.simulator");
		__consumer = __session.CreateConsumer(__destinationSim);
		__producer = __session.CreateProducer(__destinationBot);
		__consumer.Listener += new MessageListener(OnMessage);
	}

	protected void OnMessage(IMessage msg)
	{
		ITextMessage m = (ITextMessage)msg;
		BasicMessage.BotAction action = (BasicMessage.BotAction)Enum.Parse(typeof(BasicMessage.BotAction),m.NMSType);
		switch(action){
		case BasicMessage.BotAction.SHOOT:
			bool firing = false;
			Boolean.TryParse (m.Text, out firing);
			__bot.SetFiring(firing);
			break;
		case BasicMessage.BotAction.TORQUE:
			Vector3 torque = JsonUtility.FromJson<Vector3> (m.Text);
			__bot.ApplyTorque (torque);
			break;
		case BasicMessage.BotAction.FORCE:
			Vector3 force = JsonUtility.FromJson<Vector3> (m.Text);
			__bot.ApplyForce (force);
			break;
		
		}
	}

	public void SendPositionUpdate(Vector3 vector) {
		IMessage messagePos = __session.CreateTextMessage (JsonUtility.ToJson (vector));
		messagePos.NMSType = BasicMessage.BotEvent.POSITION.ToString();
		__producer.Send (messagePos);
	}

	public void SendRotationUpdate(Quaternion quat) {
		IMessage messageRot = __session.CreateTextMessage (JsonUtility.ToJson (quat));
		messageRot.NMSType = BasicMessage.BotEvent.ROTATION.ToString();
		__producer.Send (messageRot);
	}

	public void SendHpUpdate(float hp) {
		IMessage messageHp = __session.CreateTextMessage (hp.ToString());
		messageHp.NMSType = BasicMessage.BotEvent.HP.ToString();
		__producer.Send (messageHp);
	}

	public void SendTempUpdate(float temp) {
		IMessage messageHeat = __session.CreateTextMessage (temp.ToString());
		messageHeat.NMSType = BasicMessage.BotEvent.HEAT.ToString();
		__producer.Send (messageHeat);
	}

	public void SendScanUpdate(ScanMessage msg) {
		try {
			IMessage messageScan = __session.CreateTextMessage (JsonUtility.ToJson (msg));
			messageScan.NMSType = BasicMessage.RadarEvent.SCAN.ToString();
			__producer.Send (messageScan);
		} catch (Exception ex) {
		
		}
	}

	public void SendMatchUpdate(MatchStateMessage msg, BasicMessage.MatchEvent state) {
		IMessage messageScan = __session.CreateTextMessage (JsonUtility.ToJson (msg));
		messageScan.NMSType = state.ToString();
		__producer.Send (messageScan);
		Debug.Log ("Sent start");
	}
}
