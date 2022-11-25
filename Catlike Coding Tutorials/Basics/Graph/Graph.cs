using UnityEngine;

public class Graph : MonoBehaviour{
    [SerializeField] private Transform cube;
    [SerializeField, Range(10, 100)] private int resolution = 10;

    private int lastResolution;
    private Transform[] points;

    private void Awake(){
        BuildGraph();
    }

    private void Update(){
        UpdateGraph();
    }

    private void OnValidate(){
        if (Application.isPlaying && lastResolution != resolution){
            lastResolution = resolution;
            BuildGraph();
        }
    }

    private static float Function(float x){
        //return Mathf.Pow(((x - 1) % 2) - 1, 3);
        return Mathf.Sin(x * Mathf.PI);
    }

    private void UpdateGraph(){
        Vector3 position = Vector3.zero;
        float time = Time.time;
        foreach (Transform point in points){
            position.x = point.localPosition.x;
            position.y = Function(position.x + time);
            point.localPosition = position;
        }
    }

    public void BuildGraph(){
        points = new Transform[resolution];
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        float step = 2f / resolution;
        Vector3 position = Vector3.zero;
        Vector3 scale = Vector3.one * step;
        for (int i = 0; i < resolution; i++){
            Transform point = Instantiate(cube, transform, false);
            position.x = (i + 0.5f) * step - 1f;
            position.y = Function(position.x);
            point.localPosition = position;
            point.localScale = scale;
            points[i] = point;
        }
    }
}