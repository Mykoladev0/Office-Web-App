import React, {Component} from 'react';
import { GridContainer, GridItem } from '../../../../Vendor/mr-pro/components/Grid';

export class Sample extends Component<any, any> {

    public render() {
        return(
            <div>
                <GridContainer>
                    <GridItem xs={12} sm={12} md={8}>
                    </GridItem>
                </GridContainer>
            </div>
        );
    }
}
