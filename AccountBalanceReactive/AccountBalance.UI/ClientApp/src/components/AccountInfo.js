import React, { Component } from 'react';
import {AccountDashboard} from './AccountDashboard';
import {OverdraftLimit} from './OverdraftLimit';
import {DailyWireTransferLimit} from './DailyWireTransferLimit';
import { DepositCheque } from './DepositCheque';
import { DepositCash } from './DepositCash';
import { WithdrawCash } from './WithdrawCash';
import { WireTransfer } from './WireTransfer';

export class AccountInfo extends Component {
    
    state = {
        accountId: '',
    }

    togglePanels = (panelName) => {
        //console.log(panelName);
        var panels = document.getElementsByClassName("actionPanel");
        for(var i = 0; i < panels.length; i++)
        {
            panels[i].style.display = "none";
        }
        if(panelName != "")
        {
            var panel = document.getElementById(panelName);
            panel.style.display = "inline-block";
        }
        //this.forceUpdate();
    };

    handleBackClick = () => {
        this.props.showAccountsPanel(true);
        this.togglePanels("initialPanel");
    }
    
    render(){
        let component;
        if(this.props.account.length === 0)
        {
            component = null;
        }
        else {
            // console.log("above dashboard");
            // console.log(this.props.account);
            component = 
            <div>
                <AccountDashboard  account={this.props.account}/>
                <div>
                    <div className="accountaction">
                        <ul>
                            <a onClick={() => this.togglePanels("overdraftLimit")}><li>Overdraft Limit</li></a>
                            <a onClick={() => this.togglePanels("wiretransferLimit")}><li>Daily Wiretransfer Limit</li></a>
                            <a onClick={() => this.togglePanels("depositCheque")}><li>Deposit Cheque</li></a>
                            <a onClick={() => this.togglePanels("depositCash")}><li>Deposite Cash</li></a>
                            <a onClick={() => this.togglePanels("withdrawCash")}><li>Withdraw Cash</li></a>
                            <a onClick={() => this.togglePanels("wiretransfer")}><li>Wire Transfer</li></a>
                        </ul>
                    </div>
                    <div id="initialPanel" className="actionPanel" style={{display: 'inline-block'}}>
                    <table className='table table-accounts-action'>
                        <tbody>
                            <tr style={{height: '278px'}}>
                                <td style={{textAlign:'center'}}><h4>Select from left menu...</h4></td>
                            </tr>
                        </tbody>
                    </table>
                    </div>
                    <div id="overdraftLimit" className="actionPanel" style={{display: 'none'}}>
                        <OverdraftLimit account={this.props.account}/>
                    </div>
                    <div id="wiretransferLimit" className="actionPanel" style={{display: 'none'}}>
                        <DailyWireTransferLimit account={this.props.account}/>
                    </div>
                    <div id="depositCheque" className="actionPanel" style={{display: 'none'}}>
                        <DepositCheque account={this.props.account}/>
                    </div>
                    <div id="depositCash" className="actionPanel" style={{display: 'none'}}>
                        <DepositCash account={this.props.account}/>
                    </div>
                    <div id="withdrawCash" className="actionPanel" style={{display: 'none'}}>
                        <WithdrawCash account={this.props.account} refreshAccountsData={this.props.refreshAccountsData}/>
                    </div>
                    <div id="wiretransfer" className="actionPanel" style={{display: 'none'}}>
                        <WireTransfer account={this.props.account} refreshAccountsData={this.props.refreshAccountsData}/>
                    </div>
                </div>
            </div>
        }
        return(
            <div>
                <div className='backtoaccounts'><a onClick={() => this.handleBackClick()}>&lt;&lt; Back to all accounts</a></div>
                {component}
            </div>
        );
    }
}