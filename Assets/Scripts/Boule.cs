using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Objet représentant une boule contrôlée par le joueur.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boule : MonoBehaviour
{
    [SerializeField, Tooltip("La cible pour le suvi de la caméra")]
    private Transform cibleCamera;

    [SerializeField, Tooltip("Force de déplacement de la boule.")]
    private float forceDeplacement;

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    /// <summary>
    /// État de la balle
    /// </summary>
    private bool estLancee = false;

    /// <summary>
    /// Nombre de charges d'accélération
    /// </summary>
    public int nbCharges { get; private set; }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
    }

    private void Update()
    {
        if (cibleCamera != null)
        {
            cibleCamera.position = rigidbody.position;
        }

        if (!estLancee)
        {
            Commencer();
        } 
        else
        {
            return;
        }

        Accelerer();
    }

    private void FixedUpdate()
    {
        Diriger();
    }

    private void CommencerDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee += contexte.ReadValue<float>() * forceDeplacement * Vector3.right;
    }

    private void ArreterDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee = Vector3.zero;
    }

    private void Diriger()
    {
        if(!Mathf.Approximately(forceAppliquee.sqrMagnitude, 0.0f))
        {
            rigidbody.AddForce(forceAppliquee, ForceMode.Force);
        }
    }

    private void Commencer()
    {
        if (estLancee)
            return;

        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controle = ControleurJeu.Instance.Controles;

        if (controle == null)
            return;

        controle.actions.FindAction("Commencer").performed += LancerBalle;
    }

    private void LancerBalle(InputAction.CallbackContext contexte)
    {
        if (estLancee)
            return;

        // Pas de vérifications pcq déjà vérifiées dans Commencer
        ControleurJeu.Instance.Controles.actions.FindAction("Diriger").performed += CommencerDirection;
        ControleurJeu.Instance.Controles.actions.FindAction("Diriger").canceled += ArreterDirection;

        rigidbody.useGravity = true;
        estLancee = true;
    }

    public void AjouterCharge()
    {
        if (nbCharges >= 3)
            return;

        nbCharges++;

        Debug.Log(nbCharges);
    }

    public void Accelerer()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controle = ControleurJeu.Instance.Controles;

        if (controle == null)
            return;

        controle.actions.FindAction("Accelerer").performed += Accelereration;
    }

    public void Accelereration(InputAction.CallbackContext contexte)
    {
        if (!estLancee)
            return;

        if (nbCharges == 0)
            return;

        Debug.Log("Acceleration");
        rigidbody.AddForce(transform.forward * 15, ForceMode.Acceleration);
        nbCharges--;
    }
}
