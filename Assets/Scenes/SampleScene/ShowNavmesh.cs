using UnityEngine;
using UnityEngine.AI;

public class ShowNavmesh : MonoBehaviour
{
	void Start()
	{
		ShowMesh();
	}

    void Update() {
        ShowMesh();
    }

	// Generates the NavMesh shape and assigns it to the MeshFilter component.
	void ShowMesh()
	{
		// NavMesh.CalculateTriangulation returns a NavMeshTriangulation object.
		NavMeshTriangulation meshData = NavMesh.CalculateTriangulation();

        // Create a new mesh and chuck in the NavMesh's vertex and triangle data to form the mesh.
        Mesh mesh = new Mesh
        {
            vertices = meshData.vertices,
            triangles = meshData.indices
        };

        // Assigns the newly-created mesh to the MeshFilter on the same GameObject.
        GetComponent<MeshFilter>().mesh = mesh;
	}
}