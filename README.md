# CS_425_Final

Video: https://www.youtube.com/watch?v=teeIpIO9y-I&feature=youtu.be

Game Play

In this game, the player manages a pirate ship that is attempting to sail across the ocean. The challenges in this include managing your crew’s health, food supply, money, ship repairs, and overall standing with other peoples that may be encountered. This game is played entirely with the mouse. To select button options, the player must left click on the button and release the button while still hovering over button. To issues orders to crew members, the player must left click on the crew member to select them and right click to issue the order. In this game, the player is playing the job of manager of the ship, the A.I. controls much of how agents accomplish the tasks that the player gives them.

The player starts the game by entering a seed for their game or generating one randomly. This seed determines which events are at specific locations on the map. Entering the same seed twice will produce the same map, but the exact same playthrough is not guaranteed as A.I. decisions are affected by choices the player made earlier on. Once a seed is selected, the player is given a small starting crew. The stats for the crew (and all people encountered) are randomly generated. The player can reassign the crew to any positions in the ship. The positions on the ship are as follows: captain, navigator, cannoneers, doctor, and carpenter. As the ship progresses through sectors, the crew is trained at their job and their stats relating to their assigned position increase.


Captain

•	Required to be assigned to travel between sectors

•	Contributes to ships speed

•	Uses both intelligence and dexterity


Navigator

•	Increases ships vision range

•	Contributes to ships speed (not as much as captain though)

•	Uses intelligence


Cannoneer

•	Fires cannons at enemy ships during bombardment phase

•	Engages in melee combat during boarding phase

•	Uses dexterity


Doctor

•	Heals crew members when travelling between sectors

•	Uses intelligence


Carpenter

•	Automatically fixes holes in ship from cannon barrages

•	Repairs ships overall health between sectors if wood is available

•	Uses dexterity


To proceed to the next zone, the player must click the map button and select a next valid zone to travel to. Travelling between zones costs the player food per each crew member of the ship. The zones are as follows:


Early Game Zones


Empty Zone

•	Nothing happens, area is free of danger and benefits


Message in a Bottle Asking for a Rescue

•	Find a message in a bottle asking for rescue from an island

•	Player has option to decline rescue request

•	If player chose to help, can result in either rescuing people from island or it being a trap

•	Advances players ahead 1 sector


Message in a Bottle with Information of Treasure

•	Find a message in a bottle telling of treasure hidden on an island

•	Player has option to decline to look for treasure or to look for it

•	If player chose to look, can result in either finding treasure or it being a trap

•	Advances player ahead 1 sector


Trap

•	Only results from message in bottle zones

•	Player arrives at island and the inhabitants attack the player


Late Game Zones


Fishing Ship

•	Encounter a fishing ship

•	Player can choose to attack, engage in trade, or ignore it

•	Won’t attack player unless in self defense

•	Usually has more food than gold and a small crew


Merchant Ship

•	Encounter a merchant ship

•	Player can choose to attack, engage in trade, or ignore it

•	Won’t attack player unless in self defense

•	Usually has more gold than food and a moderate sized crew


Royal Navy Ship

•	Encounter military ship belonging to Royal Navy

•	Player can choose to attack, try to ignore, or engage in trade

•	Might attack player if player has been harming Royal Navy ships or civilian ships

•	Large crew size


Armada Ship

•	Encounter military ship belonging to the Armanda

•	Player can choose to attack, try to ignore, or engage in trade

•	Might attack player if player has been harming Armanda ships or civilian ships

•	Large crew size

When encountering other ships, the player has the option to engage it in combat, try to ignore it, or try to trade with it. When encountering civilian ships, the player will always be able to ignore it. However, successful trading and attacking are not always guaranteed. The player will only be able to successfully attack another ship if their ship is faster than encountered ship or if they do a surprise attack (a surprise attack occurs when the player claims they wish to trade with another ship and attacks them instead). Vision range also plays a role in this by: if the player has a higher vision range than the other ship (they can see them first), they will get a buff to whatever decision they choose to go with. The chances of engaging in trade are affected by the players trustworthiness. The player starts the game with this as a neutral stat and is either increases or decreases by the choices they make. Attacking enemies decreases it and engaging in peaceful trade increases it. When engaging in trade, the player will be able to buy goods from the other ship and attempt to gain their crew members. Additionally, the player can threaten the other ship into giving them their goods for free. The other ship will either accept this or attack the player. This decision is based on the players lethality (lethality is a measurement of how well the player has done in the past at wiping out entire ships). This also decreases trust. The player can attempt to gain crew members from the other ship in two ways. The first is by persuading the crew members to join the player. This uses the captain’s intelligence. The second is by attempting to bribe the crew members. To bribe, the player must enter an amount of money that they think the crew member will accept, if they do, they lose the money and gain the crew member, if they don’t, they lose the chance to gain the crew member but keep the money. The player only gets one chance to try to get a person to join their crew. Once the player has successfully gained a crew member from the other ship, they will no longer be able to threaten the ship.

Encountering military ships works slightly differently than encountering civilian ships. In addition to trust and lethality, the player also has a stat that measures their friendliness with the Armada and the Royal Navy. In this world, the Royal Navy and the Armada are at war with each other. Therefore, being friendly towards one reduces your influence with the other. Likewise, attacking one increases your influence with the other. Since these are military ships, if the player has disproportionately favored one side in the war, the opposing will attack the player on sight. Contrary to that, the side that the player has been favoring will always offer trading to the player. While it is hard to defeat one, it is far from impossible. The following section will discuss combat.

Combat with ships has two phases. These phases are the bombardment phase and the boarding phase. During the bombardment phase, the cannoneers of both ships fire the cannons at the other ship in the hopes to sink the other ship. The cannoneers have a chance to miss the shot and cannonball does not hit. This is based on the cannoneers dexterity. When a cannonball hits a tile, the tile becomes damaged. Damaged tiles let water flow into the ship. When this happens, a new role can be assigned to: the bailer. The bailer will run over to the water and remove it from the ship. If the entire ship fills with water, it sinks, and the player loses. To prevent this from happening the hole needs to be closed. The carpenter will automatically move to holes in the ship and patch them up. Patching up a hole also returns some of the ship’s health. Unlike the repairs between zones, this does not take up any wood to do. If both ships survive the bombardment phase. The boarding phase is started (this is also the starting phase of combat for a surprise attack). During this phase, the cannoneers stop firing the cannons and begin trying to board the other ship and perform melee attacks on the opposing crew. The attacks will continue to happen until one entire crew is either dead or the player chooses to flee. If the player completely kills the other crew, they will gain lethality. If the player flees, they will lose lethality. Additionally, if no more cannoneers remain for one ship’s crew, the cannoneers that are still alive will go up to crew members at their station and attack them. Crew members being attacked will be unable to perform their job and turn to attack the person attacking them. This will continue to happen until either the crew member is dead of the invading party has been completely killed off.

The player must keep all this in mind while playing the game in order to maximize the chances of them surviving sailing across the ocean. This game is difficult to beat and will require a couple of play throughs to get used to the mechanics and consequences of decisions the player makes.


Technical Component

For this game, I chose procedural content generation and A.I. as my technical component. The procedural content generation can be seen by choosing a seed for the game and playing it. Choosing the same seed and playing the game the exact same way will produce the same playthrough. I accomplish this by using an L-system and the seed the player entered to create the map the player uses. In this L-system, the zones labeled Early Game Zones appear more towards the beginning, then transitions into all zones, and finally transitions into only Late Game Zones towards the end of the map.
	
A.I. is a major part of this game. The A.I. manages accomplishing all the tasks the player issued it. This includes: pathing to locations, target selection, optimizing tiles to repair, optimizing tiles to remove water from, and enemy decisions. The pathing to locations is accomplished using A*. The A.I. re-paths when it is blocked by another agent. However, when no proper path can be found, and the agent is not in combat, the A.I. walks on top of other agents (this is intended because bottlenecks were not fun to play with and agents could create a block that traps all of them at once). Target selection, tile repair selection, and removing water from a tile selection is all decided by closest available choice. Lastly, enemy decisions are made based on the player previous behavior and slight randomness to it (it wasn’t fun having guarantees of what would happen all the time, the chance of random outcome as opposed to expected is usually around 10%~). Factors that discussed this are mentioned at length in the game play section. Several formulas regarding not only how the A.I. makes decisions but also how almost everything in the game works can be found in formulas.xlsx in Pirate/Assets
