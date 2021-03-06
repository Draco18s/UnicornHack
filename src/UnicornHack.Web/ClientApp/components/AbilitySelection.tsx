﻿import * as React from 'React';
import { action, computed } from 'mobx';
import { observer } from 'mobx-react';
import { Ability } from '../transport/Model';
import { GameQueryType } from '../transport/GameQueryType';
import { PlayerAction } from "../transport/PlayerAction";
import { DialogData } from '../transport/DialogData';
import { coalesce } from '../Util';
import { Dialog } from './Dialog';
import { IGameContext } from './Game';

export const AbilitySelectionDialog = observer((props: IAbilitySelectionProps) => {
    const { data, context } = props;
    return <Dialog context={context} show={computed(() => data.abilitySlot != null)}>
        <AbilitySelection {...props} />
    </Dialog>;
});

const AbilitySelection = observer(({ context, data }: IAbilitySelectionProps) => {
    if (data.abilitySlot == null) {
        throw "Rendered AbilitySelection with no data";
    }

    var abilities = Array.from(data.slottableAbilities.values(),
        i => <AbilitySelectionLine ability={i} slot={coalesce(data.abilitySlot, -3)} key={i.id} context={context} />);

    abilities.push(<AbilitySelectionLine ability={null} slot={coalesce(data.abilitySlot, -3)} key={-1} context={context} />);

    return <div className="abilitySlotSelection" role="dialog" aria-labelledby="abilitySelection">
        <h4 id="abilitySelection">Select ability for slot {data.abilitySlot}:</h4>
        <br />
        <ul>{abilities}</ul>
    </div>;
});

interface IAbilitySelectionProps {
    data: DialogData;
    context: IGameContext;
}

@observer
class AbilitySelectionLine extends React.Component<IAbilityLineProps, {}> {
    @action.bound
    setAbilitySlot(event: React.KeyboardEvent<HTMLAnchorElement> | React.MouseEvent<HTMLAnchorElement>) {
        if (event.type == 'click' || (event as React.KeyboardEvent<HTMLAnchorElement>).key == 'Enter') {
            this.props.context.showDialog(GameQueryType.Clear);
            this.props.context.performAction(
                PlayerAction.SetAbilitySlot, this.props.ability === null ? 0 : this.props.ability.id, this.props.slot);
        }
    }

    @action.bound
    showAttributes(event: React.MouseEvent<HTMLAnchorElement>) {
        if (this.props.ability !== null) {
            this.props.context.showDialog(GameQueryType.AbilityAttributes, this.props.ability.id);
        }
        event.preventDefault();
    }

    render() {
        var name = "none";
        if (this.props.ability !== null) {
            name = this.props.ability.name;
            const abilitySlot = this.props.ability.slot;

            if (abilitySlot !== null) {
                name = `[${(abilitySlot + 1)}] ` + name;
            }

            if (abilitySlot == this.props.slot) {
                return <li>{name}</li>;
            }
        }

        return <li><a tabIndex={(this.props.ability === null ? 0 : 100 + this.props.ability.id)} role="button"
            onClick={this.setAbilitySlot} onKeyPress={this.setAbilitySlot} onContextMenu={this.showAttributes}
        >
            {name}
        </a></li>;
    }
}

interface IAbilityLineProps {
    slot: number;
    ability: Ability | null;
    context: IGameContext;
}