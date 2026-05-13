import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Slide {
  tag: string;
  title: string;
  description: string;
  points: string[];
  cta: string;
  icon: string;
  accentColor: string;
  bgClass: string;
}

@Component({
  selector: 'app-hero-slider',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hero-slider.html',
  styleUrl: './hero-slider.css',
})
export class HeroSlider implements OnInit, OnDestroy {
  currentIndex = 0;
  private timer: ReturnType<typeof setInterval> | null = null;

  slides: Slide[] = [
    {
      tag: 'i dont know what to put here',
      title: 'why',
      description:
        'IDK',
      points: [
        'i will say random stuff',
        '',
        'Publish and go live in minutes',
      ],
      cta: 'Create a campaign',
      icon: 'fi fi-rr-megaphone',
      accentColor: '#c4622d',
      bgClass: 'slide-bg--terracotta',
    },
    {
      tag: 'For Donors',
      title: 'Find a cause that moves you',
      description:
        'Browse campaigns from verified organisations. Filter by category, cause type, or location — and give in the way that suits you.',
      points: [
        'Donate money or physical items',
        'Filter by cause & organisation',
        'Track every contribution you make',
      ],
      cta: 'Browse campaigns',
      icon: 'fi fi-rr-hand-holding-heart',
      accentColor: '#4a7c59',
      bgClass: 'slide-bg--moss',
    },
    {
      tag: 'Transparency',
      title: 'Every rand tracked. Every item logged.',
      description:
        'Donors see exactly where contributions go. Organisations get real-time dashboards. Nothing falls through the cracks.',
      points: [
        'Real-time donation feed',
        'Item donation tracking',
        'Full campaign analytics',
      ],
      cta: 'See how it works',
      icon: 'fi fi-rr-chart-histogram',
      accentColor: '#8b5e3c',
      bgClass: 'slide-bg--amber',
    },
    {
      tag: 'Trust & Safety',
      title: 'Verified organisations. Secure giving.',
      description:
        'Every organisation on the platform is verified. Payments are encrypted, payouts are fast, and your data is always protected.',
      points: [
        'Verified org profiles',
        'Encrypted secure payments',
        'Transparent payout process',
      ],
      cta: 'Learn about safety',
      icon: 'fi fi-rr-lock',
      accentColor: '#4a7c59',
      bgClass: 'slide-bg--green',
    },
  ];

  ngOnInit(): void {
    this.timer = setInterval(() => this.next(), 5500);
  }

  ngOnDestroy(): void {
    if (this.timer) clearInterval(this.timer);
  }

  next(): void {
    this.currentIndex = (this.currentIndex + 1) % this.slides.length;
  }

  prev(): void {
    this.currentIndex = (this.currentIndex - 1 + this.slides.length) % this.slides.length;
    this.resetTimer();
  }

  goTo(i: number): void {
    this.currentIndex = i;
    this.resetTimer();
  }

  private resetTimer(): void {
    if (this.timer) clearInterval(this.timer);
    this.timer = setInterval(() => this.next(), 5500);
  }

  getPrev(): number {
    return (this.currentIndex - 1 + this.slides.length) % this.slides.length;
  }
}