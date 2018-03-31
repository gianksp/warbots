using System;
using UnityEngine;

[Serializable]
public class MatchStateMessage {

	//About the bot
	public ScanObject metadata;

	//Match state
	public BasicMessage.MatchEvent state;
}
