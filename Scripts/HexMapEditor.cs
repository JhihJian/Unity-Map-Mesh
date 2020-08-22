using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityTemplateProjects;

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
    //检测拖拽
    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

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
        if (Input.GetMouseButton(0) ) {
            HandleInput();
        }
        else
        {
            previousCell = null;
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.Log("HandleInput");
        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.Log("Raycast hit");
            HexCell currentCell = hexGrid.GetCell(hit.point);
            //检测拖拽
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    //判断是否是有效拖拽
    void ValidateDrag(HexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        )
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
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
        Debug.Log("EditCell");
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
            //添加或移除河流
            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }
            else if (isDrag && riverMode == OptionalToggle.Yes)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    otherCell.SetOutgoingRiver(dragDirection);
                }
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
