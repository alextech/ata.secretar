using System;
using Newtonsoft.Json;
using Range = SharedKernel.Range;

namespace Ata.Investment.Profile.Domain.Profile
{
    public class TimeHorizon : ICloneable
    {
        [JsonProperty("WithdrawTime")]
        private int _withdrawTime = -1;

        [JsonProperty("Range")]
        private Range _range = new Range();

        [JsonIgnore]
        public Range Range
        {
            get => _range;
            set
            {
                _range = value;

                if (_withdrawTime == -1)
                {
                   _withdrawTime = ThirdOfPeriod();
                }
                TimeHorizonChanged?.Invoke();
            }
        }

        [JsonProperty]
        public int Origin { get; init; }

        [JsonIgnore]
        public int WithdrawTime
        {
            get => _withdrawTime;
            set
            {
                if (value+Origin > Range.Max || value+Origin < Range.Min)
                {
                    throw new ArgumentException(
                        $"Withdraw time {value} of third amount is outside the range [{Range.Max}, {Range.Min}].");
                }

                _withdrawTime = value;
                TimeHorizonChanged?.Invoke();
            }
        }

        [JsonIgnore]
        public int WithdrawYear
        {
            get => _withdrawTime + Origin;
            set => WithdrawTime = value - Origin;
        }

        public event Action? TimeHorizonChanged;

        public TimeHorizon(int origin)
        {
            Origin = origin;
        }

        private int ThirdOfPeriod() {
            if (Range.Max < Range.Min)
            {
                throw new ArgumentException($"From {Range.Max} should not be less than to {Range.Min}");
            }

            if (Origin > Range.Min || Origin > Range.Max)
            {
                throw new ArgumentException(
                    $"Neither {Range.Max} nor {Range.Min} should be less than {Origin}. Timehorizon from-to is not relative.");
            }

            return (int)Math.Floor((Range.Max - Range.Min) / 3.0) + Range.Min - Origin;
        }

        public void ResetThirdWithdrawTime()
        {
            _withdrawTime = ThirdOfPeriod();
            TimeHorizonChanged?.Invoke();
        }

        public object Clone()
        {
            return new TimeHorizon(Origin)
            {
                _withdrawTime = _withdrawTime,
                _range = (Range) _range.Clone()
            };
        }
    }
}
