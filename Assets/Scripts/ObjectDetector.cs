using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] private TowerSpawner towerSpawner;
    [SerializeField] private TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                if(hit.transform.CompareTag("Tile"))
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
                else if(hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                towerDataViewer.OffPanel();
            }

            hitTransform = null;
        }
    }
}
