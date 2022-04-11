using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOWScripts : MonoBehaviour
{
    public GameObject m_FOWPlane;
    public Transform[] m_Player;
    public LayerMask m_fogLayer;
    public float m_radius = 5f;
    private float m_radiusSqr { get { return m_radius * m_radius; } }

    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private Color[] m_colors;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        GameObject[] Player = GameObject.FindGameObjectsWithTag("Player");
        m_Player = new Transform[Player.Length];
        for (int i = 0; i < m_Player.Length; i++)
        {
            m_Player[i] = Player[i].transform;
        }

        foreach (Transform player in m_Player)
        {
            Ray r = new Ray(transform.position, player.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, m_fogLayer, QueryTriggerInteraction.Collide))
            {
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    Vector3 v = m_FOWPlane.transform.TransformPoint(m_vertices[i]);
                    float dist = Vector3.SqrMagnitude(v - hit.point);
                    if (dist < m_radiusSqr)
                    {
                        float alpha = Mathf.Min(m_colors[i].a, dist / m_radiusSqr);
                        m_colors[i].a = alpha;
                    }
                }
                UpdateColor();
            }
        }
        
    }

    void Initialize()
    {
        m_mesh = m_FOWPlane.GetComponent<MeshFilter>().mesh;
        m_vertices = m_mesh.vertices;
        m_colors = new Color[m_vertices.Length];
        for(int i = 0; i < m_colors.Length; i++)
        {
            m_colors[i] = Color.black;
        }
        UpdateColor();
    }

    void UpdateColor()
    {
        m_mesh.colors = m_colors;
    }
}
