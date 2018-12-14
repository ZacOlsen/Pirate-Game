
public class ArmadaShipEvent : FactionShipEvent {

	protected override float factionTrust {
		set { playerShip.AddInFavorOfArmada (value - factionTrust); }
		get { return -playerShip.factionTrust; }
	}

	protected override string factionName { get { return ARMADA; } }
	protected override string otherFactionName { get { return ROYAL_NAVY; } }

	protected override PlayerShip.PactStatus factionStatus { 
		set { playerShip.armadaStatus = value; } 
		get { return playerShip.armadaStatus; }
	} 

	protected override PlayerShip.PactStatus otherFactionStatus { 
		set { playerShip.royalNavyStatus = value; } 
		get { return playerShip.royalNavyStatus; }
	}
}
