using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using System.Globalization;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using System.Diagnostics;
using System.CodeDom.Compiler;

public class ttt_ai : Agent
{
    public GameController AgameController;
    public string type;
    public int gameCount = 0;

    public override void CollectObservations(VectorSensor sensor)
    {
        for (int i=0;i<9;i++)
        {
            sensor.AddObservation(AgameController.ObserveState(type, i));
        }
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {            
        actionMasker.SetMask(0, AgameController.GetOccupiedFields());
    }

    public override void OnEpisodeBegin()
    {
        var team = GetComponent<BehaviorParameters>().TeamId;
        if (team == 0 && gameCount==0) type = "X";
        if (team == 1 && gameCount==0) type = "O";
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        AgameController.SelectField(Mathf.FloorToInt(vectorAction[0]), type);
    }

    public void SwitchSide()
    {
        if (type == "X")
        {
            type = "O";
        }
        else
        {
            type = "X";
        }
    }
}
