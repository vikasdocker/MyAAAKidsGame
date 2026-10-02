using System.Text;
using UnityEngine;

namespace Dab.Runtime.Creature
{
    /// <summary>
    /// Drag-to-feed. NOT enabled by default, deliberately.
    ///
    /// Why this state is opt-in
    /// ------------------------
    /// game-concept.md section 8 records the research constraint directly: at
    /// age 5 a child "lacks fine motor control and object persistence for
    /// drag-to-feed". Feeding as a drag-to-target verb is therefore an
    /// assumption this project has not validated with its actual audience, and
    /// the concept doc lists it as a question to resolve, not a decided mechanic.
    ///
    /// The state is built because the framework needs the slot and because the
    /// mechanic may well be right for the older end of the 6-9 band. What is
    /// deliberately NOT built is a way to reach it accidentally. There is no
    /// input path from Painting or Idle into Feeding, and no feeding item in the
    /// scene. Enabling it is a deliberate act:
    ///
    ///     machine.TransitionTo(CreatureStateId.Feeding)
    ///
    /// once a feeding affordance exists and the age-5 drag concern has been
    /// checked against playtest evidence. Building the state early while
    /// leaving it unreachable keeps the option open at close to zero cost and
    /// avoids shipping an unvalidated verb to the youngest players.
    ///
    /// If feeding is later reworked into a tap-to-offer interaction (which the
    /// age-5 constraint pushes toward), the hook set here is what such a rework
    /// would need: OnOffer, OnConsume, OnRefuse. It is written as a
    /// drag-shaped interaction now so the change is visible if it happens.
    /// </summary>
    public sealed class FeedingState : CreatureState
    {
        /// <summary>
        /// Seconds a drag must stay within the target before it counts as an
        /// offer rather than a pass-by. Generous, per the input manager's
        /// forgiveness rules, but a drag that merely crosses the creature should
        /// not be interpreted as feeding it.
        /// </summary>
        private const float MinOfferSeconds = 0.25f;

        /// <summary>Seconds the eat animation runs before the creature is content.</summary>
        private const float EatSeconds = 0.8f;

        /// <summary>Hunger rises over this many seconds from content to eager.</summary>
        private const float HungerRampSeconds = 300f;

        private float _heldSeconds;
        private float _eatElapsed;
        private float _hunger01;
        private bool _eating;

        public override CreatureStateId Id => CreatureStateId.Feeding;

        public override string DisplayName => "Feeding";

        public override void OnEnter()
        {
            _heldSeconds = 0f;
            _eatElapsed = 0f;
            _eating = false;
            OnAnimatorTrigger("Offer_Reach");
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_eating)
            {
                _eatElapsed += deltaTime;

                if (_eatElapsed >= EatSeconds)
                {
                    _eating = false;
                    OnAnimatorTrigger("Content_Settle");
                }

                return;
            }

            _heldSeconds += deltaTime;

            if (_heldSeconds >= MinOfferSeconds)
            {
                BeginEating();
            }

            // Hunger is a pure function of time in this stub. It is exposed so a
            // future save or tuning system has a place to drive it from, and so
            // the Animator can already lean on it. Nothing in art-bible 5.x
            // forbids a slightly more eager creature as time passes, and it is
            // the natural way to make the return hook read as anticipation rather
            // than as neglect (game-concept.md, "never punish absence").
            _hunger01 = Mathf.Clamp01(_hunger01 + deltaTime / HungerRampSeconds);
            OnAnimatorFloat("Hunger", _hunger01);
        }

        private void BeginEating()
        {
            _eating = true;
            _eatElapsed = 0f;
            _hunger01 = 0f;
            OnAnimatorTrigger("Eat");
        }

        public override void OnTouchEnded(Vector2 screenPosition, bool wasTap)
        {
            if (!_eating)
            {
                // Lifted before the offer completed. The creature simply looks
                // up. No penalty, no fail state: the art bible is consistent
                // that nothing in this product is a punishment.
                OnAnimatorTrigger("Look_Up");
            }

            _heldSeconds = 0f;
        }

        public override void OnExit()
        {
            _eating = false;
            _heldSeconds = 0f;
        }

        public override TransitionResult CanTransitionTo(CreatureStateId target, CreatureStateMachine machine)
        {
            // Eating is uninterruptible for the same reason painting is: a
            // child who takes a bite and watches the creature stop chewing has
            // been given a broken promise. 0.8s is short enough that this is not
            // a real wait.
            return _eating ? TransitionResult.BlockedBusy : TransitionResult.Allowed;
        }

        public override void Describe(StringBuilder builder)
        {
            builder.Append(DisplayName)
                   .Append(_eating ? " eating" : " offering")
                   .Append(" hunger=").Append(_hunger01.ToString("F2"));
        }
    }
}
