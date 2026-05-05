using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class Actor : MonoBehaviour
{
	[SerializeField] public string actorName = "Actor";
	
	private Character character;
	private CharacterBrain characterBrain;

	public Character Character() => character;
	public CharacterBrain Brain() => characterBrain;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		// character = GetComponent<Character>();
		// var characterManager = GameManager.Instance.GetSystem<CharacterManager>();
		//
		// characterBrain = characterManager.GetCharacter(actorName);
		//
		// characterManager.RegisterActor(this);
		//
		// if (character.IsPlayer)
		// {
		// 	characterManager.RegisterPlayerActor(this);
		// }
	}




	private void OnDestroy()
	{
		//GameManager.Instance.GetSystem<CharacterManager>().UnregisterActor(this);
	}
}
