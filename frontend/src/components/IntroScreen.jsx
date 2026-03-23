import { useEffect, useState } from "react";

export default function IntroScreen({ onReady }) {
  const [showText, setShowText] = useState(false);
  const [canProceed, setCanProceed] = useState(false);
  const [fadeOut, setFadeOut] = useState(false);

  useEffect(() => {
    // Show "Are you ready?" after a short delay
    const textTimer = setTimeout(() => setShowText(true), 800);

    // Allow proceeding after showing text for a bit
    const proceedTimer = setTimeout(() => setCanProceed(true), 2800);

    return () => {
      clearTimeout(textTimer);
      clearTimeout(proceedTimer);
    };
  }, []);

  const handleProceed = () => {
    if (canProceed && !fadeOut) {
      setFadeOut(true);
      // Wait for fade animation to complete before calling onReady
      setTimeout(() => onReady(), 600);
    }
  };

  return (
    <main className={`intro-page ${fadeOut ? "fade-out-intro" : ""}`}>
      {/* Animated curves background */}
      <svg className="curves-background" viewBox="0 0 1200 800">
        {/* Curve 1 - Top left flowing */}
        <path
          className="curve curve-1"
          d="M -100,150 Q 300,50 600,100 T 1300,150"
          fill="none"
          stroke="url(#gradient1)"
          strokeWidth="3"
        />

        {/* Curve 2 - Middle flowing */}
        <path
          className="curve curve-2"
          d="M -50,400 Q 250,300 600,350 T 1250,400"
          fill="none"
          stroke="url(#gradient2)"
          strokeWidth="3"
        />

        {/* Curve 3 - Bottom flowing */}
        <path
          className="curve curve-3"
          d="M -100,650 Q 300,750 600,700 T 1300,650"
          fill="none"
          stroke="url(#gradient3)"
          strokeWidth="3"
        />

        {/* Animated orbs */}
        <circle
          className="orb-1"
          cx="200"
          cy="150"
          r="40"
          fill="rgba(255,183,102,0.6)"
        />
        <circle
          className="orb-2"
          cx="1000"
          cy="400"
          r="50"
          fill="rgba(174,220,255,0.5)"
        />
        <circle
          className="orb-3"
          cx="300"
          cy="700"
          r="35"
          fill="rgba(174,220,255,0.4)"
        />

        {/* Gradient definitions */}
        <defs>
          <linearGradient id="gradient1" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" stopColor="#ffc266" stopOpacity="0.3" />
            <stop offset="50%" stopColor="#ff9950" stopOpacity="0.6" />
            <stop offset="100%" stopColor="#ff7733" stopOpacity="0.3" />
          </linearGradient>
          <linearGradient id="gradient2" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" stopColor="#a5d4ff" stopOpacity="0.4" />
            <stop offset="50%" stopColor="#6ba3ff" stopOpacity="0.6" />
            <stop offset="100%" stopColor="#a5d4ff" stopOpacity="0.4" />
          </linearGradient>
          <linearGradient id="gradient3" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" stopColor="#ffd4a3" stopOpacity="0.3" />
            <stop offset="50%" stopColor="#ffb366" stopOpacity="0.5" />
            <stop offset="100%" stopColor="#ffd4a3" stopOpacity="0.3" />
          </linearGradient>
        </defs>
      </svg>

      {/* Content overlay */}
      <div className="intro-content">
        <div className={`intro-text ${showText ? "visible" : ""}`}>
          <h1>Are you ready?</h1>
          <p>Let's manage your queue efficiently</p>
        </div>

        {canProceed && (
          <button
            className="intro-button"
            onClick={handleProceed}
            disabled={fadeOut}
          >
            Get Started
          </button>
        )}
      </div>
    </main>
  );
}
