// tslint:disable:class-name
import React, { Component, ComponentType } from 'react';
import throttle from 'lodash/fp/throttle';

export interface withGetScreenOptions {
  mobileLimit: number;
  tabletLimit: number;
  shouldListenOnResize?: boolean;
}

export const defaultOptions: withGetScreenOptions = {
  mobileLimit: 468,
  tabletLimit: 768,
  shouldListenOnResize: true,
};

export interface withGetScreenState {
  currentSize: ScreenType;
}

export enum ScreenType {
  MOBILE,
  TABLET,
  DESKTOP,
}

export function withGetScreen<T>(
  WrappedComp: ComponentType<T>,
  options = defaultOptions
): ComponentType {
  return class extends Component<T, withGetScreenState> {
    public constructor(props: T, context: any) {
      super(props, context);
      //   this.resize = this.resize.bind(this);
      this.onResize = throttle(100, this.onResize);
      this.state = {
        currentSize: this.getSize(window.innerWidth),
      };
    }
    public componentDidMount() {
      if (options.shouldListenOnResize) {
        window.addEventListener('resize', this.onResize);
      }
    }
    public componentWillUnmount() {
      this.onResize.cancel();
      window.removeEventListener('resize', this.onResize);
    }

    public isMobile = () => {
      return this.state.currentSize === ScreenType.MOBILE;
    };
    public isTablet = () => {
      return this.state.currentSize === ScreenType.TABLET;
    };
    public isDesktop = () => {
      return this.state.currentSize === ScreenType.DESKTOP;
    };
    public render() {
      const detectMethods = {
        isMobile: this.isMobile,
        isTablet: this.isTablet,
        isDesktop: this.isDesktop,
      };
      return <WrappedComp {...detectMethods} {...this.props} />;
    }
    private onResize: any = () => {
      const newSize = this.getSize(window.innerWidth);
      if (newSize !== this.state.currentSize) {
        this.setState({
          currentSize: newSize,
        });
      }
    };
    private getSize(size: number): ScreenType {
      if (size <= options.mobileLimit) {
        return ScreenType.MOBILE;
      } else if (size >= options.tabletLimit) {
        return ScreenType.DESKTOP;
      } else {
        return ScreenType.TABLET;
      }
    }
  };
}
