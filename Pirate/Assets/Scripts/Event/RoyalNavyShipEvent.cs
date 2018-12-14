using UnityEngine;

public class RoyalNavyShipEvent : FactionShipEvent {

	protected override float factionTrust {
		set { playerShip.AddInFavorOfRoyalNavy (value - factionTrust); }
		get { return playerShip.factionTrust; }
	}

	protected override string factionName { get { return ROYAL_NAVY; } }
	protected override string otherFactionName { get { return ARMADA; } }

	protected override PlayerShip.PactStatus factionStatus { 
		set { playerShip.royalNavyStatus = value; } 
		get { return playerShip.royalNavyStatus; }
	}

	protected override PlayerShip.PactStatus otherFactionStatus { 
		set { playerShip.armadaStatus = value; } 
		get { return playerShip.armadaStatus; }
	} 
}
