﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    int activeElevation;
    //是否应用颜色
    bool applyColor;
    //是否应用高度
    bool applyElevation = true;
    //笔刷大小
    int brushSize;
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    //河流的编辑模式，忽略河流，添加或删除河流。
    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    OptionalToggle riverMode;


    void Awake () {
        SelectColor(0);
    }

    void Update () {
        if (Input.GetMouseButton(0)) {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }
    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }


    void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (applyColor)
            {
                cell.Color = activeColor;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
        }
    }
    public void SelectColor (int index) {
        applyColor = index >= 0;
        if (applyColor)
        {
            activeColor = colors[index];
        }
    }
    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }
    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }
    //设置河流的编辑模式
    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }
}
