﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Global : MonoBehaviour
{

    // Main Map object with all informations about current map states
    private Assets.Scripts.Map.Map gameMap;
    private ObjectRenderer objectRenderer = new ObjectRenderer();
    public List<Assets.Scripts.Map.MapObject> listOfMapObjects { get; set; } 
    public Transform prefab_grass, prefab_water, prefab_sand, prefab_archer, prefab_swordsman, prefab_mutant, prefab_horseman, prefab_castle;

    private int userTurn = 0;

    private int UserTurn 
    {
        get{ return userTurn; }
        set
        {
            userTurn = value;
        }
    }



    private Assets.Scripts.Map.MapObject highlightedObject;
   
    public Assets.Scripts.Map.MapObject ListOfMapObjects
    {
        get
        {
            return this.ListOfMapObjects;
        }
    }
    public Assets.Scripts.Map.Map GameMap{
        get{
            return this.gameMap;
        }
    }

    public void HandleMapElementClick(Vector2 mapPos)
    {
        if (highlightedObject==null)
        {
            Debug.Log("no figure is selected");
            return;
        }

        var mapObjectAtPos = listOfMapObjects.FindLast(obj => (float)obj.x == mapPos.x && (float)obj.y == mapPos.y);

        if (mapObjectAtPos != null)
        {
            // something is standing in this place
            Debug.Log("something is standing in this place");
            return;
        }
        highlightedObject.x = (int)mapPos.x;
        highlightedObject.y = (int)mapPos.y;
        objectRenderer.UpdateObjects();
    }

    public void HandleFigureHighlight(Assets.Scripts.Map.MapObject selectedObj)
    { 
        if (selectedObj == null)
        {
            Debug.LogError("Selected object not found in the list");
            return;
        }

        if (selectedObj == highlightedObject)
        {
            Debug.Log("Unselecting object");
            highlightedObject.isHighlighted = false;
            highlightedObject = null;
            objectRenderer.UpdateObjects();
            return;
        }
        if ( selectedObj.ownerID != userTurn)
        {
            // player wants to select the unit he doesn't own 
            return;
        }
        highlightedObject = selectedObj;
        foreach(var obj in listOfMapObjects)
        {
            obj.isHighlighted = false;
        }
        highlightedObject.isHighlighted = true;

        objectRenderer.UpdateObjects();

    }

    // Use this for initialization
    void Start () {
        listOfMapObjects= new List<Assets.Scripts.Map.MapObject>();
        var filter= gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        this.gameMap = Assets.Scripts.Map.MapLoader.LoadMapFromJson(Assets.Scripts.Map.GlobalMapConfig.JsonMapPath);

        //set prefabs for objectRenderer
        objectRenderer.setPrefabs(prefab_archer, prefab_swordsman, prefab_mutant, prefab_horseman, prefab_castle);

        var mapRenderer = new MapRenderer(GameMap, prefab_grass, prefab_water, prefab_sand, listOfMapObjects, objectRenderer);
        mapRenderer.RenderTheMap();
        UserTurn = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void endTurn()
    {
        var numberOfPlayers = 2;
        this.UserTurn = (this.UserTurn + 1) % numberOfPlayers;
        foreach (var obj in listOfMapObjects)
        {
            obj.isHighlighted = false;
        }
        objectRenderer.UpdateObjects();
        Debug.Log(this.userTurn);
    }
}
