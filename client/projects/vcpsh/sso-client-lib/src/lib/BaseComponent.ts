import {OnDestroy, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';

export class BaseComponent implements OnInit, OnDestroy {
  private _subscriptions: Subscription[] = [];
  private _onInit: (() => void)[] = [];
  private _onDestroy: (() => void)[] = [];

  protected addSub(...subs: Subscription[]) {
    this._subscriptions.push(...subs);
  }

  protected addOnInit(...fns: (() => void)[]) {
    this._onInit.push(...fns);
  }

  protected addOnDestroy(...fns: (() => void)[]) {
    this._onDestroy.push(...fns);
  }

  public ngOnDestroy(): void {
    this._onDestroy.forEach(fn => fn());
    this._subscriptions.forEach(s => s.unsubscribe());
  }

  public ngOnInit(): void {
    this._onInit.forEach(fn => fn());
  }
}
