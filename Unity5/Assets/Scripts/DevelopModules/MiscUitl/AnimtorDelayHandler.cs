using UnityEngine;
using System.Collections;

public class AnimtorDelayHandler : MonoBehaviour
{
    [SerializeField] //DebugSet
    private float m_counter;

    public float DelaySeconds = 0.5f;

    private Animator m_animator;
    private int m_defaultStateHash;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_defaultStateHash = m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
    }

    private void OnEnable()
    {
        if (!m_animator) return;
        //
        if (m_animator.enabled)
        {
            m_animator.speed = 0;
            m_animator.Play(m_defaultStateHash, 0, 0);
            m_counter = 0;
        }

    }

    private void Update()
    {
        if (!m_animator) return;
        //
        m_counter += Time.deltaTime;

        if (m_counter >= DelaySeconds) {

            m_animator.speed = 1;
            m_animator.Play(m_defaultStateHash, 0, 0);

            this.enabled = false;
        }

    }

}
