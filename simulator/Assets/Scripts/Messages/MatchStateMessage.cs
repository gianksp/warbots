using System;
using UnityEngine;

[Serializable]
public class MatchStateMessage {

	//About the bot
	public ScanMessage metadata;

	//Match state
	public BasicMessage.MatchEvent state;
}
