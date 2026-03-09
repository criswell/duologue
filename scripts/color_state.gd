## Tetradic color system from the original Duologue.
## Three color pairs (positive/negative), each with light/medium/dark variants.
## Players and enemies use these colors for gameplay mechanics.

enum Variant { LIGHT, MEDIUM, DARK }
enum Polarity { POSITIVE, NEGATIVE }

# Three color states, each a pair of complementary colors.
# Index: [state][polarity][variant]
const COLORS := [
	# State 0: Blue / Orange
	{
		Polarity.POSITIVE: {  # Blue
			Variant.LIGHT:  Color(0.0, 0.8, 1.0),
			Variant.MEDIUM: Color(0.0, 0.56, 0.7),
			Variant.DARK:   Color(0.0, 0.32, 0.4),
		},
		Polarity.NEGATIVE: {  # Orange
			Variant.LIGHT:  Color(1.0, 0.68, 0.0),
			Variant.MEDIUM: Color(0.7, 0.48, 0.0),
			Variant.DARK:   Color(0.4, 0.27, 0.0),
		},
	},
	# State 1: Green / Red
	{
		Polarity.POSITIVE: {  # Green
			Variant.LIGHT:  Color(0.0, 1.0, 0.73),
			Variant.MEDIUM: Color(0.0, 0.7, 0.51),
			Variant.DARK:   Color(0.0, 0.4, 0.29),
		},
		Polarity.NEGATIVE: {  # Red
			Variant.LIGHT:  Color(1.0, 0.0, 0.26),
			Variant.MEDIUM: Color(0.7, 0.0, 0.18),
			Variant.DARK:   Color(0.4, 0.0, 0.1),
		},
	},
	# State 2: Yellow / Violet
	{
		Polarity.POSITIVE: {  # Yellow
			Variant.LIGHT:  Color(1.0, 0.97, 0.0),
			Variant.MEDIUM: Color(0.7, 0.68, 0.0),
			Variant.DARK:   Color(0.4, 0.39, 0.0),
		},
		Polarity.NEGATIVE: {  # Violet
			Variant.LIGHT:  Color(0.54, 0.0, 1.0),
			Variant.MEDIUM: Color(0.38, 0.0, 0.7),
			Variant.DARK:   Color(0.22, 0.0, 0.4),
		},
	},
]

const STATE_COUNT := 3


static func get_color(state: int, polarity: int, variant: int) -> Color:
	return COLORS[state % STATE_COUNT][polarity][variant]


static func opposite_polarity(p: int) -> int:
	return Polarity.NEGATIVE if p == Polarity.POSITIVE else Polarity.POSITIVE
