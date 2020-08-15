﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class HexCell : MonoBehaviour
{
    [SerializeField]
    HexCell[] neighbors ;
    public int elevation;
    public HexCoordinates coordinates;
    public Color color;
    public RectTransform uiRect;
    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }


    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            //对整个单元格做高度的扰动
            position.y +=
    (HexMetrics.SampleNoise(position).y * 2f - 1f) *
    HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = elevation * -HexMetrics.elevationStep;
            //对整个单元格做高度的扰动
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;
        }
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }
}
