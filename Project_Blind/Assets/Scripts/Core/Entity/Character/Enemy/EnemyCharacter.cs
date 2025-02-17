using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blind
{
    public class EnemyCharacter : Character
    {
        protected CharacterController2D _characterController2D;
        protected Rigidbody2D rigid;
        protected SpriteRenderer _sprite;
        protected CapsuleCollider2D _col;

        [SerializeField] protected ScriptableObjects.EnemyCharacter Data;

        protected GameObject player;
        public MeleeAttackable _attack;
        public LayerMask WallLayer;
        public Transform WallCheck;
        protected Transform startPosition;
        protected Vector2 patrolDirection;
        protected PlayerFinder playerFinder;
        protected EnemyAttack attackSense;

        // HP UI
        protected UI_UnitHP _unitHPUI = null;
        
        //콜백 함수 델리게이트들
        public delegate void onDeath();

        public onDeath DeathCallback = () => {};

        protected virtual void Awake()
        {
            base.Awake(Data);
            _attack = GetComponent<MeleeAttackable>();
            _sprite = GetComponent<SpriteRenderer>();
            _characterController2D = GetComponent<CharacterController2D>();
            rigid = GetComponent<Rigidbody2D>();
            CreateHpUI(Data.HpBarHeight);
            playerFinder = GetComponentInChildren<PlayerFinder>();
            _col = GetComponent<CapsuleCollider2D>();
        }

        protected void CreateHpUI(float BarHeight)
        {
            // UI매니저로 UI_UnitHP 생성
            _unitHPUI = UIManager.Instance.ShowWorldSpaceUI<UI_UnitHP>();
            // UI에서 UnitHP 참조
            _unitHPUI.HP = Hp;
            // 유닛 움직이면 같이 움직이도록 Parent 설정
            _unitHPUI.transform.SetParent(transform);
            // UI에서 이 오브젝트의 정보가 필요할 수도 있으므로 참조
            _unitHPUI.Owner = gameObject;
            // 오브젝트의 머리 위에 위치하도록 설정
            _unitHPUI.SetPosition(transform.position, Vector3.up * BarHeight);
        }

        public override Facing GetFacing()
        {
            if (transform.localScale.x > 0)
                return Facing.Right;
            else return Facing.Left;
        }

        public override void HitSuccess()
        {
            return;
        }

        protected override void onHurt()
        {
            return;
        }

        public virtual void Reset()
        {
            return;
        }

        protected override void HurtMove(Facing enemyFacing)
        {
            _characterController2D.Move(new Vector2((float)enemyFacing*1, 0));
            return;
        }
        
    }
}