import './styles/site.scss';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as mobx from 'mobx';
import { Game } from './components/Game';

function renderApp() {
    mobx.configure({ enforceActions: 'observed', computedRequiresReaction: true });

    const paramString = new URL(window.location.toString()).search.slice(1);
    let nextParamIndex = paramString.indexOf('?');
    if (nextParamIndex === -1) {
        nextParamIndex = paramString.length;
    }
    const playerName = new URLSearchParams(paramString.slice(0, nextParamIndex)).get("Name") || '';
    const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;
    if (playerName !== '') {
        ReactDOM.render(
            <Game playerName={playerName} baseUrl={baseUrl} />,
            document.getElementById('react-app')
        );
    }
}

renderApp();
