import React from 'react';

import capitalize from 'lodash/fp/capitalize';

import FormControl from "@material-ui/core/FormControl/FormControl";
import InputLabel from "@material-ui/core/InputLabel/InputLabel";
import Select from "@material-ui/core/Select/Select";
import MenuItem from "@material-ui/core/MenuItem/MenuItem";
import { WithStyles, withStyles } from "@material-ui/core/styles";

// style for this view
import customSelectStyle from "../../../Vendor/mr-pro/assets/jss/material-dashboard-react/customSelectStyle.jsx";
import FormHelperText from '@material-ui/core/FormHelperText/FormHelperText';
import Input from '@material-ui/core/Input/Input';

export interface GenderDropdownProps extends WithStyles<typeof customSelectStyle> {
    prompt?: string;
    handleSelectionFn: (selectedGender: any) => void;
    value?: any;
    required: boolean;
    readonly: boolean;
}

const genderChoices: any[] = [
    {title:"Male", value:"Male"},
    {title:"Female", value:"Female"},
    {title:"Unknown", value:"Unknown"}
];

// const getGenderStringFromMenuValue = (val: number): string => {
//     switch(Number(val)) {
//         case 1:
//         return "";
//         case 2:
//         return "Male";
//         case 3:
//         return "Female";
//         case 4:
//         return "Unknown";
//     }
//     return "";
// };

const GenderDropdownComp = (props: GenderDropdownProps) => {
    const {classes, prompt, handleSelectionFn, value, required, readonly} = props;
    const promptLabel: string = (prompt && prompt.length > 0)?prompt:"Choose Gender";
    //want readonly for a property in the first input element
    const inputCtrl = readonly?(<Input name="gender" id="gender" inputProps={{readOnly:true}} />):(<Input name="gender" id="gender"/>);
    //need to handle when ??? comes from the db
    const formattedValue = value?(value as string).includes("?")?"Unknown":value:value;
    return (
        <FormControl
            required={required}
            disabled={readonly}
            fullWidth
            className={classes.selectFormControl}
        >
            <InputLabel
                htmlFor="simple-select"
                className={classes.selectLabel}>{promptLabel}
            </InputLabel>
            <Select
                MenuProps={{
                    className: classes.selectMenu
                }}
                classes={{
                    select: classes.select
                }}
                value={capitalize(formattedValue)}
                onChange={(evt: any)=> {
                    const gender = evt.target.value;
                    handleSelectionFn(gender);
                }}
                input={inputCtrl}
                inputProps={{
                    name: "genderSelect",
                    id: "gender-select",
                }}
            >
            <MenuItem
                disabled
                classes={{
                    root: classes.selectMenuItem
                }}
            >{promptLabel}</MenuItem>
            {genderChoices.map(g=> {
                return (
                    <MenuItem
                        classes={{
                            root: classes.selectMenuItem,
                            selected: classes.selectMenuItemSelected
                        }}
                        key={g.value}
                        value={g.title}>{g.value}
                    </MenuItem>
                );
            })}
             </Select>
            {required &&
                <FormHelperText>Required</FormHelperText>
            }
        </FormControl>
    );
};

const GenderDropDown = withStyles(customSelectStyle)(GenderDropdownComp);
export {GenderDropDown};
