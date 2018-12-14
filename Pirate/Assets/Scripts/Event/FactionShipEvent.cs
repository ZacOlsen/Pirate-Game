using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FactionShipEvent : ShipEvent {

	public Button pactButton;
	public GameObject engageWarning;
	protected string previousMessage;

	private System.Action methodAction;
	private Warning warning;

	protected abstract float factionTrust { set; get; }

	protected abstract string factionName { get; }
	protected abstract string otherFactionName { get; }

	protected abstract PlayerShip.PactStatus factionStatus { set; get; } 
	protected abstract PlayerShip.PactStatus otherFactionStatus { set; get; } 

	protected const string ROYAL_NAVY = "The Royal Navy";
	protected const string ARMADA = "The Armada";

	protected const float TRUST_FACTION_ACTION_DELTA = .2f;

	private enum Warning {
		ATTACK,
		NOT_ATTACK,
		THREATEN
	}

	protected new void Start () {

		base.Start ();

		if (factionStatus == PlayerShip.PactStatus.FORMED) {
			pactButton.interactable = false;
		}
	}

	protected override ShipDecision GenerateShipDecision (bool offerBonus) {

		PlayerShip.PactStatus status = factionStatus;
		float trust = playerShip.trustworthiness;

		if (status == PlayerShip.PactStatus.FORMED) {
			return ShipDecision.PARLEY;

		} else if (status == PlayerShip.PactStatus.BROKEN) {
			return ShipDecision.ENGAGE;
		}

		float parleyBreakpoint = 125f / (1f + Mathf.Exp (3f * factionTrust - (.5f - playerShip.trustworthiness))) - 7f;
		float engageBreakpoint = 125f / (1f + Mathf.Exp (3f * factionTrust - (.5f - playerShip.trustworthiness))) - 15f;

		float point = Random.Range (0f, 100f);
		if (offerBonus) {
			point += 5f;
		}

		if (point > parleyBreakpoint) {
			return ShipDecision.PARLEY;
		} else if(point < engageBreakpoint) {
			return ShipDecision.ENGAGE;
		}

		return Random.Range (0, 2) > 0 ? ShipDecision.PARLEY : ShipDecision.ENGAGE;
	}

	protected override bool IsThreatenSuccessful () {

		float engageBreakpoint = -playerShip.lethality * 130f + 140f;
		float giveBreakpoint = -playerShip.lethality * 130f + 130f;

		float point = Random.Range (0f, 100f);

		if (point > giveBreakpoint) {
			return true;
		} else if (point < engageBreakpoint) {
			return false;
		}

		return Random.Range (0, 2) > 0;
	}

	public override void ContinueMessage () {
		Engage ();
	}

	public new void Engage () {

		if (factionStatus == PlayerShip.PactStatus.FORMED) {
			EngageWarning ();
			methodAction = Engage;
			warning = Warning.ATTACK;
			return;
		}

		base.Engage ();
		factionTrust -= TRUST_FACTION_ACTION_DELTA;
		
		//readd trust if must attack by pact
		if (factionStatus == PlayerShip.PactStatus.BROKEN || otherFactionStatus == PlayerShip.PactStatus.FORMED) {
			playerShip.AddTrust (TRUST_ACTION_DELTA);
		}
	}

	public new void AttemptToEngage () {

		if (factionStatus == PlayerShip.PactStatus.FORMED) {
			EngageWarning ();
			methodAction = Engage;  //do real engage on this
			warning = Warning.ATTACK;
			return;
		}

		base.AttemptToEngage ();
		if (actionSuccess) {
			factionTrust -= TRUST_FACTION_ACTION_DELTA;
			//readd trust if must attack by pact
			if (factionStatus == PlayerShip.PactStatus.BROKEN || otherFactionStatus == PlayerShip.PactStatus.FORMED) {
				playerShip.AddTrust (TRUST_ACTION_DELTA);
			}
		}
	}

	public new void AcceptParley () {

		if (otherFactionStatus == PlayerShip.PactStatus.FORMED) {
			NonEngageWarning ();
			methodAction = AcceptParley;
			warning = Warning.NOT_ATTACK;
			return;
		}

		base.AcceptParley ();
	}
	
	public new void AttemptToParley () {

		if (otherFactionStatus == PlayerShip.PactStatus.FORMED) {
			NonEngageWarning ();
			methodAction = AttemptToParley;
			warning = Warning.NOT_ATTACK;
			return;
		}

		base.AttemptToParley ();
	}

	public new void AttemptSurpriseAttack () {

		if (factionStatus == PlayerShip.PactStatus.FORMED) {
			EngageWarning ();
			methodAction = SurpriseAttack;
			warning = Warning.ATTACK;
			return;
		}

		base.AttemptSurpriseAttack ();
	}
	
	public new void SurpriseAttack () {

		Debug.Log ("this");

		if (factionStatus == PlayerShip.PactStatus.FORMED) {
			EngageWarning ();
			methodAction = SurpriseAttack;
			warning = Warning.ATTACK;
			return;
		}

		base.SurpriseAttack ();

		factionTrust -= TRUST_FACTION_ACTION_DELTA;
		//readd trust if must attack by pact
		if (factionStatus == PlayerShip.PactStatus.BROKEN || otherFactionStatus == PlayerShip.PactStatus.FORMED) {
			playerShip.AddTrust (TRUST_ACTION_DELTA);
		}
	}

	public new void Ignore () {

		if (otherFactionStatus == PlayerShip.PactStatus.FORMED) {
			NonEngageWarning ();
			methodAction = Ignore;
			warning = Warning.NOT_ATTACK;
			return;
		}

		base.Ignore ();
		if (actionSuccess) {
			factionTrust += TRUST_FACTION_ACTION_DELTA;
		}
	}

	public new void ThreatenEngagement () {

		if (factionStatus == PlayerShip.PactStatus.FORMED) {
			goodsMenu.SetActive (false);
			gameObject.SetActive (true);
			ThreatenWarning ();
			methodAction = GoThroughWithThreaten;
			warning = Warning.THREATEN;
			return;
		}

		base.ThreatenEngagement ();
		factionTrust -= TRUST_FACTION_ACTION_DELTA;
	}

	private void GoThroughWithThreaten () {
		gameObject.SetActive (false);
		goodsMenu.SetActive (true);
		base.ThreatenEngagement ();
		factionTrust -= TRUST_FACTION_ACTION_DELTA;
	}

	public void OfferPact () {

		if (factionTrust >= .4f && factionTrust + playerShip.trustworthiness / 2f >= .8f) {
			info.text = "They have accepted";
			factionStatus = PlayerShip.PactStatus.FORMED;
			factionTrust += TRUST_FACTION_ACTION_DELTA;
		} else {
			info.text = "They did not accept";
		}

		Invoke ("ClearInfoText", 3);
		pactButton.interactable = false;
	}

	public void ResponseToPrompt (bool response) {

		if (!response) {

			if (warning == Warning.THREATEN) {
				DeclineThreaten ();
				return;
			}

			DeclineWarning ();
			return;
		}

		switch (warning) {
			case Warning.ATTACK:
			case Warning.THREATEN:
				if (factionStatus == PlayerShip.PactStatus.FORMED) {

					factionStatus = PlayerShip.PactStatus.BROKEN;
					factionTrust -= TRUST_FACTION_ACTION_DELTA;
					playerShip.AddTrust (-TRUST_ACTION_DELTA);
				}
				break;

			case Warning.NOT_ATTACK:
				if (otherFactionStatus == PlayerShip.PactStatus.FORMED) {

					otherFactionStatus = PlayerShip.PactStatus.BROKEN;
					factionTrust += TRUST_FACTION_ACTION_DELTA;
					playerShip.AddTrust (-TRUST_ACTION_DELTA);
				}
				break;
			}

		methodAction ();
	}

	private void ThreatenWarning () {
		ShowWarning ();
		message.text = "Threatening " + factionName + " will break our pact with them. Do you want to do this?";
	}

	private void EngageWarning () {
		ShowWarning ();
		message.text = "Engaging " + factionName + " will break our pact with them. Do you want to do this?";
	}

	private void NonEngageWarning () {
		ShowWarning ();
		message.text = "Not engaging " + factionName + " will break our pact with " + otherFactionName + 
			". Do you want to do this?";
	}

	private void ShowWarning () {
		
		previousMessage = message.text;
		engageWarning.SetActive (true);

		if (PlayerHasRangeAdvantage ()) {
			playerSeesFirstOptions.SetActive (false);

		} else {
			shipSeesFirstOptions.SetActive (false);
		}
	}

	private void DeclineWarning () {

		message.text = previousMessage;
		engageWarning.SetActive (false);

		if (PlayerHasRangeAdvantage ()) {
			playerSeesFirstOptions.SetActive (true);

		} else {
			shipSeesFirstOptions.SetActive (true);
		}
	}

	private void DeclineThreaten () {
		gameObject.SetActive (false);
		goodsMenu.SetActive (true);
	}
}
