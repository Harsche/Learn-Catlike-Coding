using UnityEngine;
using System.Collections.Generic;

public class TransformationGrid : MonoBehaviour {
    [SerializeField] private Transform point;
    [SerializeField] private int gridResolution;
    private List<Transformation> transformations;
    private Transform[] grid;
    private Matrix4x4 transformation;

    private void Awake() {
        GenerateGrid();
        transformations = new List<Transformation>();
    }

    private void Update() {
        UpdateTransformation();
        TransformPoints();
    }

    private void GenerateGrid() {
        grid = new Transform[gridResolution * gridResolution * gridResolution];
        for (int i = 0, z = 0; z < gridResolution; z++)
            for (int y = 0; y < gridResolution; y++)
                for (int x = 0; x < gridResolution; x++, i++)
                    grid[i] = GenerateGridPoint(x, y, z);
    }

    private Transform GenerateGridPoint(int x, int y, int z) {
        Transform point = Instantiate<Transform>(this.point);
        point.SetParent(transform);
        point.localPosition = GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / gridResolution,
            (float)y / gridResolution,
            (float)z / gridResolution
        );
        return point;
    }

    private Vector3 GetCoordinates(int x, int y, int z) {
        return new Vector3(
            x - (gridResolution - 1) * 0.5f,
            y - (gridResolution - 1) * 0.5f,
            z - (gridResolution - 1) * 0.5f
        );
    }

    private void TransformPoints() {
        for (int i = 0, z = 0; z < gridResolution; z++)
            for (int y = 0; y < gridResolution; y++)
                for (int x = 0; x < gridResolution; x++, i++)
                    grid[i].localPosition = TransformPoint(x, y, z);
    }

    private Vector3 TransformPoint(int x, int y, int z) {
        Vector3 coordinates = GetCoordinates(x, y, z);
        return transformation.MultiplyPoint(coordinates);
    }

    private void UpdateTransformation() {
        GetComponents<Transformation>(transformations);
        if (transformations.Count > 0) {
            transformation = transformations[0].Matrix;
            for (int i = 1; i < transformations.Count; i++) {
                transformation = transformations[i].Matrix * transformation;
            }
        }
    }
}
