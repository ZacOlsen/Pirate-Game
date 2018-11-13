using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterPeopleEvent : EmptyZoneEvent {

	private static readonly string[] FIRST_NAMES = { "Will", "Oliver", "Wex", "Mark", "Bill", "Carlos", "Baron",
		"Sebastian", "Larry", "Gregory", "Gout", "Newt", "Kieth", "Pete", "Gerald", "Pedro", "Don", "Joe",
		"Christopher", "Jamal"
	};

	private static readonly string[] LAST_NAMES = { "McKey", "Swaller", "Schmidt", "Ford", "Forman", "Yeager",
		"Vallo", "Ward", "Avery", "Douglas", "Black", "Stapleton", "Gruda", "Gumbo", "Crimson", "Jenkins",
		"Horne", "Watson", "Smith", "Saunders"
	};

	public const int AVERAGE_STARTING_DEXTERITY = 8;
	public const int AVERAGE_STARTING_INTELLIGENCE = 8;
	public const int AVERAGE_STARTING_HEALTH = 12;
	public const int MAX_HEALTH = 20;

	[SerializeField] private int dexDelta = 0;
	[SerializeField] private int intDelta = 0;
	[SerializeField] private int hpDelta = 0;

	[SerializeField] private float dexStandardDev = 2;
	[SerializeField] private float intStandardDev = 2;
	[SerializeField] private float hpStandardDev = 4;

	[SerializeField] protected int foodMin = 10;
	[SerializeField] protected int foodMax = 15;
	[SerializeField] protected int woodMin = 10;
	[SerializeField] protected int woodMax = 15;
	[SerializeField] protected int goldMin = 10;
	[SerializeField] protected int goldMax = 15;

	public GameObject crewPrefab;
	public int minNumOfPeople = 0;
	public int maxNumOfPeople = 3;

	public SelectionManager selectionManager;
	protected List<CrewMember> otherPeople = new List<CrewMember> ();

	protected CrewMember CreateCrewMember (MovementTile tile) {
		
		CrewMember c = Instantiate (crewPrefab, tile.transform.position, Quaternion.identity).GetComponent<CrewMember> ();
		c.current = tile;
		tile.crewMem = c;

		AssignBaseStats (c);

		return c;
	}

	protected void AssignBaseStats (CrewMember c) {

		int dex = Mathf.Clamp (Mathf.RoundToInt (MathStatistics.GenerateStandardNormalNumber (AVERAGE_STARTING_DEXTERITY 
			+ dexDelta, dexStandardDev)), 1, int.MaxValue);

		int intell = Mathf.Clamp (Mathf.RoundToInt (MathStatistics.GenerateStandardNormalNumber (AVERAGE_STARTING_INTELLIGENCE
			+ intDelta, intStandardDev)), 1, int.MaxValue);

		int hp = Mathf.Clamp (Mathf.RoundToInt (MathStatistics.GenerateStandardNormalNumber (AVERAGE_STARTING_HEALTH
			+ hpDelta, hpStandardDev)), 1, MAX_HEALTH);

		string cName = FIRST_NAMES[Random.Range (0, FIRST_NAMES.Length)] + " " + 
			LAST_NAMES[Random.Range (0, LAST_NAMES.Length)];

		c.SetBaseStats (cName, intell, dex, hp);
		c.role = CrewMember.Role.CANNONEER;
	}
}
