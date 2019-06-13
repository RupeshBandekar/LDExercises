import React, { Component } from 'react';
import {NewAccount} from './NewAccount';
import {Accounts} from './Accounts';
import { AccountInfo } from './AccountInfo';

export class AccountContainer extends Component {
    state= {
        loadAccountList: true,
        accountId: '',
    };
    getAccountInfo = (accountId) => {
        console.log(accountId);
    }

    toggleState = (showAccountList, newAccountId) =>{
        this.setState({loadAccountList: showAccountList,
                       accountId: newAccountId});
    }

    render(){
        let component;

        if(this.state.loadAccountList) {
            component = <div><NewAccount /><Accounts getAccountInfoMethod={this.getAccountInfo}  toggleStateMethod={this.toggleState}/></div>
        }
        else {
            component = <div><AccountInfo accountId={this.state.accountId} toggleStateMethod={this.toggleState} /></div>
        }
        return(
            <div>
                <h1>Account Balance</h1>
                {component}
            </div>
        );
    }
}