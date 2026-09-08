use std::str::FromStr;

use rust_decimal::Decimal;
use rust_decimal::RoundingStrategy;

/// nopCommerce `RoundingType` names, matching the C# enum.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum RoundingType {
    Rounding001,
    Rounding005Up,
    Rounding005Down,
    Rounding01Up,
    Rounding01Down,
    Rounding05,
    Rounding1,
    Rounding1Up,
}

impl FromStr for RoundingType {
    type Err = String;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "Rounding001" => Ok(Self::Rounding001),
            "Rounding005Up" => Ok(Self::Rounding005Up),
            "Rounding005Down" => Ok(Self::Rounding005Down),
            "Rounding01Up" => Ok(Self::Rounding01Up),
            "Rounding01Down" => Ok(Self::Rounding01Down),
            "Rounding05" => Ok(Self::Rounding05),
            "Rounding1" => Ok(Self::Rounding1),
            "Rounding1Up" => Ok(Self::Rounding1Up),
            other => Err(format!("unknown roundingType '{other}'")),
        }
    }
}

impl RoundingType {
    pub fn as_str(self) -> &'static str {
        match self {
            Self::Rounding001 => "Rounding001",
            Self::Rounding005Up => "Rounding005Up",
            Self::Rounding005Down => "Rounding005Down",
            Self::Rounding01Up => "Rounding01Up",
            Self::Rounding01Down => "Rounding01Down",
            Self::Rounding05 => "Rounding05",
            Self::Rounding1 => "Rounding1",
            Self::Rounding1Up => "Rounding1Up",
        }
    }
}

/// Port of `PriceCalculationService.Round` (C# `Math.Round(value, 2)` + cash rounding).
pub fn round(value: Decimal, rounding_type: RoundingType) -> Decimal {
    let ten = Decimal::from(10);
    let hundred = Decimal::from(100);

    let mut rez = value.round_dp_with_strategy(2, RoundingStrategy::MidpointNearestEven);
    let mut fraction_part = (rez - rez.trunc()) * ten;

    if fraction_part.is_zero() {
        return rez;
    }

    match rounding_type {
        RoundingType::Rounding005Up | RoundingType::Rounding005Down => {
            fraction_part = (fraction_part - fraction_part.trunc()) * ten;
            fraction_part %= Decimal::from(5);
            if fraction_part.is_zero() {
                return rez;
            }

            if rounding_type == RoundingType::Rounding005Up {
                fraction_part = Decimal::from(5) - fraction_part;
            } else {
                fraction_part = -fraction_part;
            }

            rez += fraction_part / hundred;
        }
        RoundingType::Rounding01Up | RoundingType::Rounding01Down => {
            fraction_part = (fraction_part - fraction_part.trunc()) * ten;

            if rounding_type == RoundingType::Rounding01Down && fraction_part == Decimal::from(5) {
                fraction_part = Decimal::from(-5);
            } else {
                fraction_part = if fraction_part < Decimal::from(5) {
                    -fraction_part
                } else {
                    Decimal::from(10) - fraction_part
                };
            }

            rez += fraction_part / hundred;
        }
        RoundingType::Rounding05 => {
            fraction_part *= ten;
            fraction_part = if fraction_part < Decimal::from(25) {
                -fraction_part
            } else if fraction_part < Decimal::from(50) || fraction_part < Decimal::from(75) {
                Decimal::from(50) - fraction_part
            } else {
                Decimal::from(100) - fraction_part
            };

            rez += fraction_part / hundred;
        }
        RoundingType::Rounding1 | RoundingType::Rounding1Up => {
            fraction_part *= ten;

            if rounding_type == RoundingType::Rounding1Up && fraction_part > Decimal::ZERO {
                rez = rez.trunc() + Decimal::ONE;
            } else {
                rez = if fraction_part < Decimal::from(50) {
                    rez.trunc()
                } else {
                    rez.trunc() + Decimal::ONE
                };
            }
        }
        RoundingType::Rounding001 => {}
    }

    rez
}

pub fn round_str(value: &str, rounding_type: &str) -> Result<String, String> {
    let value = Decimal::from_str(value).map_err(|e| e.to_string())?;
    let rounding_type = RoundingType::from_str(rounding_type)?;
    Ok(round(value, rounding_type).normalize().to_string())
}

#[cfg(test)]
mod tests {
    use super::*;

    fn d(s: &str) -> Decimal {
        Decimal::from_str(s).unwrap()
    }

    #[test]
    fn bankers_midpoints_match_csharp_to_even() {
        assert_eq!(round(d("1.225"), RoundingType::Rounding001), d("1.22"));
        assert_eq!(round(d("2.005"), RoundingType::Rounding001), d("2.00"));
        assert_eq!(round(d("2.015"), RoundingType::Rounding001), d("2.02"));
        assert_eq!(round(d("12.365"), RoundingType::Rounding001), d("12.36"));
    }

    #[test]
    fn cash_midpoints_used_by_sidecar_spawn() {
        assert_eq!(round(d("12.35"), RoundingType::Rounding005Down), d("12.35"));
        assert_eq!(round(d("12.05"), RoundingType::Rounding01Up), d("12.10"));
        assert_eq!(round(d("12.05"), RoundingType::Rounding01Down), d("12.00"));
    }

    #[test]
    fn round_str_invariant_wire() {
        assert_eq!(round_str("1.225", "Rounding001").unwrap(), "1.22");
    }

    #[test]
    fn matches_existing_canround_cases() {
        let cases: &[(&str, &str, RoundingType)] = &[
            ("12.366", "12.37", RoundingType::Rounding001),
            ("12.363", "12.36", RoundingType::Rounding001),
            ("12.000", "12.00", RoundingType::Rounding001),
            ("12.001", "12.00", RoundingType::Rounding001),
            ("12.34", "12.35", RoundingType::Rounding005Up),
            ("12.36", "12.40", RoundingType::Rounding005Up),
            ("12.35", "12.35", RoundingType::Rounding005Up),
            ("12.00", "12.00", RoundingType::Rounding005Up),
            ("12.05", "12.05", RoundingType::Rounding005Up),
            ("12.20", "12.20", RoundingType::Rounding005Up),
            ("12.001", "12.00", RoundingType::Rounding005Up),
            ("12.34", "12.30", RoundingType::Rounding005Down),
            ("12.36", "12.35", RoundingType::Rounding005Down),
            ("12.00", "12.00", RoundingType::Rounding005Down),
            ("12.05", "12.05", RoundingType::Rounding005Down),
            ("12.20", "12.20", RoundingType::Rounding005Down),
            ("12.35", "12.40", RoundingType::Rounding01Up),
            ("12.36", "12.40", RoundingType::Rounding01Up),
            ("12.00", "12.00", RoundingType::Rounding01Up),
            ("12.10", "12.10", RoundingType::Rounding01Up),
            ("12.35", "12.30", RoundingType::Rounding01Down),
            ("12.36", "12.40", RoundingType::Rounding01Down),
            ("12.00", "12.00", RoundingType::Rounding01Down),
            ("12.10", "12.10", RoundingType::Rounding01Down),
            ("12.24", "12.00", RoundingType::Rounding05),
            ("12.49", "12.50", RoundingType::Rounding05),
            ("12.74", "12.50", RoundingType::Rounding05),
            ("12.99", "13.00", RoundingType::Rounding05),
            ("12.00", "12.00", RoundingType::Rounding05),
            ("12.50", "12.50", RoundingType::Rounding05),
            ("12.49", "12.00", RoundingType::Rounding1),
            ("12.50", "13.00", RoundingType::Rounding1),
            ("12.00", "12.00", RoundingType::Rounding1),
            ("12.01", "13.00", RoundingType::Rounding1Up),
            ("12.99", "13.00", RoundingType::Rounding1Up),
            ("12.00", "12.00", RoundingType::Rounding1Up),
        ];

        for (value, expected, rounding_type) in cases {
            assert_eq!(
                round(d(value), *rounding_type),
                d(expected),
                "{value} {rounding_type:?}"
            );
        }
    }
}
